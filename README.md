# ChatChallenge

## Challenge Description

The goal of this exercise is to create a simple browser-based chat application using .NET.
This application should allow several users to talk in a chatroom and also to get stock quotes
from an API using a specific command.

## Mandatory Features

* Allow registered users to log in and talk with other users in a chatroom.
* Allow users to post messages as commands into the chatroom with the following format
/stock=stock_code
* Create a decoupled bot that will call an API using the stock_code as a parameter
(<https://stooq.com/q/l/?s=aapl.us&f=sd2t2ohlcv&h&e=csv>, here aapl.us is the
stock_code)
* The bot should parse the received CSV file and then it should send a message back into
the chatroom using a message broker like RabbitMQ. The message will be a stock quote
using the following format: “APPL.US quote is $93.42 per share”. The post owner will be
the bot.
* Have the chat messages ordered by their timestamps and show only the last 50
messages.
* Unit test the functionality you prefer.

## Bonus (Optional)

* Have more than one chatroom.
* Use .NET identity for users authentication
* Handle messages that are not understood or any exceptions raised within the bot.
* Build an installer.

## Considerations

* We will open 2 browser windows and login with 2 different users to test the
functionalities.
* The stock command won’t be saved on the database as a post.
* The project is totally focused on the backend; please have the frontend as simple as you can.
* Keep confidential information secure.
* Pay attention if your chat is consuming too many resources.
* Keep your code versioned with Git locally.
* Feel free to use small helper libraries

---

## How about the solution?

    My thoughts on this, so be kind to understand what I was thinking and feeling. 
    Not everyone knows everything or learns it in the same way.

So basically, in my opinion, this challenge is asking for a web chat application where multiple authenticated users can enter a chat room and send messages.

Also, they can send the command `/stock=stock_code`, for example: `/stock=aapl.us`, to see the amount per share of the stock.

This command should be handled by a **DECOUPLED** bot that should be able to identify the command, consume the given API, extract the data needed from a CSV file received, and finally send back a message to the chat.

So clearly I could use a message broker. The recruiters even suggest RabbitMQ which I accepted to use.

With the requirements in place, I tried to slice the problem into three parts:

1) The Chat
2) The Bot
3) The Message Broker

---

### 1. The Chat

Starting with item 1 I decided to go straight forward with a Web ASP.NET Core template to create a project with Identity and then added SignalR.

The Identity part was already built-in by the template with EF Core, so just needed a local SQL Server DB and run the migrations (Update-Database on PM console).

Added SignalR dependencies, created a new page with a very basic UI for the chat and restricted the access for logged users only.

Did some testing opening multiple browsers and so far so good!

    PS: Oh, I was almost forgetting to mention that I used Visual Studio 2022 Community.

---

### 2. The Bot

Satisfied with the web app I start thinking about item 2 and for that, I decided to create a simple Console App.

Started working on consuming the stocks API by adding the CSVHelper dependency to read the data from the CSV file and convert it to an object.

Then worked on some chat command and response validations to the bot (extracted to a class library for reuse and to keep things organized).

Did some testing on the Console App and created some unit tests for the validators.

I was happy with the result!

So far I had two independent projects, The Chat and The Bot (not a bot yet since it was not communicating with the chat).

So there was the need for a third item!

---

### 3. The Message Broker

First I would need the RabbitMQ running. So I decided to work with Docker and create a container with the management interface.

`docker run -d --hostname rabbit --name some-rabbit -p 15672:15672 -p 5672:5672 -e RABBITMQ_DEFAULT_USER=admin -e RABBITMQ_DEFAULT_PASS=admin rabbitmq:3-management`

    PS: I like Docker but I'm not an expert on it. 
    This is a very useful tool when I want to run a service without the need to install it on my machine and test some other stuff.

I confess that my experience with RabbitMQ at the moment of this challenge is low. But reading the documentation and looking at some examples on the internet I found the [MassTransit Framework](https://masstransit-project.com/).

    It is a free, open-source distributed application framework for .NET.

Reading the docs and the examples I found a [SignalR example with RabbitMQ](https://masstransit-project.com/advanced/signalr/sample.html#sample-signalr) and **boom!** That was everything I needed for the whole challenge!

I just need to configure the chat and the bot consumes the same queue and it's done!

But my lack of experience with the new framework took me to a different approach. I could make both apps look for the same queue but I don't know why my messages on the chat were not being consumed by the bot.

Looking into RabbitMQ queues on the admin interface, I saw multiple queues created by MassTransit for SignalR and another one called `'broadcast-message'` which I thought would be the queue the consumers were listening for.

I published a message manually and both consumers received it but I couldn't figure out why MassTransit was not using that queue when sending a chat message and how could make it use it or how to identify the current queue being used and configure it for both apps.

I realized that MassTransit identifies the consumers in the assembly (according to the configuration) and creates a queue based on the class name by default.

For example:

* A `StockBotConsumer` class will listen to a `stock-bot` queue.
* A `BroadcastMessageConsumer` class will listen to the `broadcast-message` queue.

With time running out and the constant increase in my anxiety, I decided to take a step back.

The chat doesn't need to use a message broker. So I removed the MassTransit SignalR dependency.

Then, in the ChatHub class, where I send messages to the chat, I decided to publish a message to the bot's queue. And I refactored the consumer class to listen to consume the messages sent by the bot.

Unfortunately, I had to leave the Contracts layer for as common use for chat and bot to exchange messages because of MassTransit.

    For sure it's not the appropriate 'fix' for this issue. 
    Probably some simple silly configuration could make it work as I thought before but at that moment I hadn't had more time to try other stuff.
    And besides the current solution is not an appropriate fix, it worked as expected.

This part consumed most of my time and the rest of it I spent refactoring and trying to accomplish some bonus requirements.

At most finals, I decided to dockerize everything to make it easier to run without too much configuration (God thanks Visual Studio for the amazing support!).

---

## Stack used

Finally, the final solution and stack used were:

For the chat:

* ChatWebApp, built with ASP.NET Core Web App + Identity + EF Core + SignalR + MassTransit + Docker.

For the bot:

* StockBot, built with Console App + MassTransit + Docker

Auxiliary Class Libraries:

* Contracts, a class library to define the message types for the projects.
* BotCommandValidator, a class library with some business rules validations, checks, and extractions for the `/stock=stock_code` command.
* BotCommandValidatorTests, a basic test project with MSTest2 to cover some command validator scenarios.

All are built with .NET 6.

For the database:

* A [SQL Server](https://hub.docker.com/_/microsoft-mssql-server) docker container.

For the message broker:

* A [RabbitMQ](https://hub.docker.com/_/rabbitmq) docker container.

## How to run?

Since the project is dockerized, I believe the easiest way to run it on your machine is by having the [latest docker version](https://www.docker.com/) installed, then:

1) Clone this repo to your local machine
2) In the terminal or prompt, navigate to the folder where you cloned the repo.
3) Type `docker-compose up` and hit enter.
