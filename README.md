# RiskIt

## Introduction

This is my fun little project where I try to build something looking like the board game RISK. This is in no shape or form built to be actually used as the game. I have taken inspiration from the board game as my imagination is not the greatest and building something with a clear roadmap (features) is easier.

In this project you can find code I am proud of, code I am less proud of, code that tries to do something cool and fails, and code that tries to do something cool and succeeds.

### Disclaimer

Everything I wrote here may or may not break at any given time or be more or less implemented. I try to write what is already included or I'm very sure I will implement but as with any good documentation - by the time it's saved it's already deprecated.

## How to run

Honestly I just use visual studio and set the startup project to ConsoleGame. I don't think this should be run in non-debug mode ;)

## What can you find

There are a few cool things to look for:

* Playback ability
    * Any game has complete replayability (forwards) due to events and using a random saved seed value for dice casts
    * Some people call this event sourcing
* Testing of randomness
    * I've implemented ways to simulate dice casts and have them be consistent such that testing parts of the system that rely on randomness is still possible
* Modular design
    * Most part of the (backend)system are quite well modulized into domain chunks. This tries to follow the principle "software that fit's in your head" by Dan North.
    * I follow a design "pattern" that goes like: "make it work, make it pretty". Thus some mess is expected and somethings I will leave "messy" because I'll probably never look at it again.
* Clear interfaces
    * Definitely not a subjective claim.
    * I try to keep interfaces clear and close to the physical world. I.e. A game client interacts with a game by saying what the user wants and receiving what the game has done.

## Where do I submit my complaint

Please write your complaint on a piece of a6 paper (very important), fold it [7 (seven) times](https://www.youtube.com/watch?v=6EQeh2aK81Q) in half.
Now locate the closest trash can, insert the paper into the trash can and live a happy life. We will get back to you before the end of times, maybe.
