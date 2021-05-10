# CavCash (CAV)
A re-implementation of the CryptoNode protocol in C# with some other goodies.

CavCash aims to be a CryptoCurrency with privacy features similar to Monero.

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

## Whitepaper
A whitepaper is in the works. Will be posted here when it's closer to the final thing. Not too concerned with getting it out quick since this is just a pet project at the moment.

## Contributing
Once the code is here, feel free to make PRs about whatever, if they're breaking or they go against some of the basic goals, it will not be merged but pretty much anything else will be.
