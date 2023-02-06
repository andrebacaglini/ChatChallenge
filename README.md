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

* We will open 2 browser windows and log in with 2 different users to test the
functionalities.
* The stock command won’t be saved on the database as a post.
* The project is totally focused on the backend; please have the frontend as simple as you
can.
* Keep confidential information secure.
* Pay attention if your chat is consuming too many resources.
* Keep your code versioned with Git locally.
* Feel free to use small helper libraries

---

## How about the solution?

    My thoughts on this so be kind to understand what I was thinking and feeling. Not everyone knows everything or learn it on the same way.

So basically, in my opinion, this challenge is asking for a web chat application where multiple authenticated users can enter to a chat room and send messages.

Also, they can send the command '`/stock=stock_code`', for example: '`/stock=aapl.us`', to see the amount per share of the stock.

This command should be handled by a **DECOUPLED** bot that should be able to identify the command, consume the given API, extract the data needed from a CSV file received, and finally send back a message to the chat.

So clearly I could use a message broker. The recruiters even suggest RabbitMQ which I accepted to use.

With the requirements in place, I tried to slice the problem in three parts:

1) The Chat
2) The Bot
3) The Message Broker

---

### 1. The Chat

Starting with the item 1 I decided to go straight forward with an Web ASP.NET Core template to create a project with Idendity and then added SignalR.

The Identity part was already built-in by the template with EF Core, so just needed a local SQL Server DB and run the migrations (Update-Database on PM console).

Added SignalR dependencies, created a new page with a very basic UI for the chat and restricted it the access for logged users only.

Did some testing opening multiple browsers and so far so good!

    PS: Oh, I was almost forgetting to mention that I used Visual Studio 2022 Community.

---

### 2. The Bot

Satisfyed with the web app I start thinking on item 2 and for that I decided to created a simple Console App.

Started working on consuming the stocks API by adding the CSVHelper dependency to read the data from the CSV file and convert to an object.

Then worked on some chat command and response validations to the bot (extracted to a class library for reuse and keep things organized).

Did some testing on the Console App and created some unit test for the validators.

I was happy with the result!

So far I had two independent projects, The Chat and The Bot (not really a bot yet since it was not communicating with the chat).

So there was the need for third item!

---

### 3. The Message Broker

First I would need the RabbitMQ running. So I decided to work with Docker and create a container with the management interface.

`docker run -d --hostname rabbit --name some-rabbit -p 15672:15672 -p 5672:5672 -e RABBITMQ_DEFAULT_USER=admin -e RABBITMQ_DEFAULT_PASS=admin rabbitmq:3-management`

    PS: I like Docker but I'm not an expert on it. This is a very useful tool when I want to run a service without need to install it on my machine and test some other stuff.

I confess that my experience with RabbitMQ at the moment of this challenge is low. But reading the documentations and looking some examples on the internet I found the MassTransit Framework.

It is a free, open-source distributed application framework for .NET (<https://masstransit-project.com/>).

Reading the docs and the examples I found a SignalR example with RabbitMQ (<https://masstransit-project.com/advanced/signalr/sample.html#sample-signalr>) and **boom!** That was everything I needed for the whole challenge!

I just need to configure the chat and the bot consume the same queue and it's done!

But my lack of experience with the new framework took me to a different approach. I could make both apps look for the same queue but I don't know why my messages on the chat was not being consumed by the bot.

Looking into RabbitMQ queues on admin interface, I saw multiple queues created by MassTransit for SignalR and another one called `'broadcast-message'` which I thought would be the queue the consumers was listening for.

I published a message manually and the both consumers received it but I couldn't figure out why MassTransit was not using that queue when sending a chat message and how could make it use it or how to identify the current queue being used and configure it for both apps.

I realized that MassTransit identifies the consumers in the assembly (according the configuration) and creates a queue based on the class name by default.

For example:

* A `StockBotConsumer` class will listen to a `stock-bot` queue.
* A `BroadcastMessageConsumer` class will listen to `broadcast-message` queue.

With time running out and the constant increase of my anxiety I decided to take a step back.

In the ChatHub class, where I send messages to the chat, I decided to send a message to the bot's queue also, and of course at least I did a basic validation to see if the message should be sent to bot's queue or not.

    For sure it's not the approprietaded 'fix' for this issue. Probably some simple silly configuration could make it work as I thought before but at that moment I hadn't more time to try other stuff and besides the current solution its not an approprietaded fix it worked as expected.

This part consumed most of my time and the rest of it I spent refactoring and trying to accomplish some bonus requirements.

---

## Stack used

Finally the final solution and stack used was:

For the chat:

* ChatWebApp, built with ASP.NET Core Web App + Identity + EF Core + SignalR + MassTransit.
* Uses local SQLServer DB to store the users and the messages.

For the bot:

* StockBot, built with Console App + MassTransit.

Auxiliary Class Libraries:

* Contracts, a class library to define the message types for the projects.
* BotCommandValidator, a class library with some business rules validations, checks and extractions for the `/stock=stock_code` command.
* BotCommandValidatorTests, a basic test project with MSTest2 to cover some command validator scenarios.

All built with .NET 6.