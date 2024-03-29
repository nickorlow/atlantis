# Atlantis (ATL) (Formerly known as CavCash)

IMPORTANT NOTE: Recently, I discovered that some of the code that validates signatures on transactions was working in a faulty manner and have not yet had time to make a patch. 

This is a seperate project from CavCash the payment processor

A re-implementation of the CryptoNode protocol in C# with some other goodies.

Atlantis aims to be a CryptoCurrency with privacy features similar to Monero.

Atlantis uses a SHA-256 proof of work hashing algorithm to keep the chain secure.

## The name
The name is a namesake to a company I ran with a few friends during high school

## Not yet launched
Atlantis has not yet launched a testnet or a production network. Currently testing is only done by running a node on localhost.

## Re-Implemented/Done Features
- Stealth Addresses
- Mining with increasing difficulty
- Send/Recieve money
- Move Node software from using our own hacky TCP server to just running as an ASP.NET WebAPI. This has considerable benefits since the folks up in Redmond have been working on making ASP performant and it also gives us a bunch of features to play with.

## To-Do Features
- Ring Signatures
- Ring Confidential Transactions
- Nakamodo consensus for multiple nodes (currently multi-node isn't a thing)
- Don't use NBitcoin library for Stealth Addresses
- Rewrite CavCash.BlockExplore to React.js/ts so it's cleaner

## Whitepaper
A whitepaper is in the works. Will be posted here when it's closer to the final thing. Not too concerned with getting it out quick since this is just a pet project at the moment.

## Contributing
Feel free to make PRs about whatever, if they're breaking or they go against some of the basic goals, it will not be merged but pretty much anything else will be.

## Components (Still using legacy naming scheme)

### CavCash.BlockExplore
Our very own (poorly designed) block explorer. Mostly used for debugging. 
#### Running
Just build and run. If you're not running a node on localhost:5001, then edit the corresponding string in `CryptoUtils.cs` on line 51

### CavCash.Wallet
A CLI wallet used mostly for testing. Running in 'epheremal mode' doesn't save the keys to disk, useful for testing.

### CavCash.Node.ASP
Re-write of unreleased CavCash.Node. Moves away from our own custom TCP server into an ASP.NET WebAPI. ASP.NET was chosen as it is pretty well built and saves us a lot of work, especially since it has SSL/TLS built in and has great performance.

### CavCash.Core
Common funcitons between all of CavCash's components.
