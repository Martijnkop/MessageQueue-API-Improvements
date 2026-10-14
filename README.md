# Learning about scaling a Software System

This repository contains my experiments around scaling software systems.

---

The context of the application is a simple social media application, with the endpoints:
| Endpoint               | Method      | Explanation                     |
|------------------------|-------------|---------------------------------|
| /posts                 | GET         | Gets the feed (latest 20 posts) |
| /posts/search          | GET / QUERY | Search the feed                 |
| /posts/{id}            | GET         | Opens a specific post           |
| /posts                 | POST        | Create a new post               |
| /posts/like/{postid}   | POST        | Like a post                     |
| /posts/reply/{postid}  | POST        | reply to a post                 |
| /users/follow/{userid} | POST        | follow a user                   |


## BASE

> /social-media

This folder contains the 'base' version of the application. This is a Domain Driven Design implementation of the endpoints. The architecture choice is overkill, but it makes it easy to convert into other architectures. This will make the development time for my experiments (cache, message queues, ...) as short as possible

| Status      |
|-------------|
| In progress |

## LOAD-TESTER

> /load-tester

This will contain the K6 tests that will benchmark the applications.

| Status      |
|-------------|
| To do |

## DB SEEDER

This will contain a simple program that will seed the database before testing. (To get valid tests, the starting databases should be the same)

| Status      |
|-------------|
| To do |