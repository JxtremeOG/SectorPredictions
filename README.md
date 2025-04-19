# SectorPredictions

Follow-up project to the Riskless portfolio optimizer from Philly Codefest 2025.  
Implements a Genetic Algorithm that evolves the hyperparameters of another Genetic Algorithm to dynamically allocate sector weights across 11 sectors of the S&P 500, with the goal of outperforming the index while minimizing risk.

## Features
- Dual-layer Genetic Algorithms implemented with Generics for modularity and reusability
- Dynamic sector allocation based on risk-adjusted returns
- Integration of momentum indicators such as RSI, ADL, and rolling returns
- Trains on 16 quarters of data from 1.1K+ S&P 500 tickers (and scalable)

## Background
Originally built during Philly Codefest 2025 and expanded for more robust backtesting, deeper market analysis, and a cleaner codebase structure.
Written in C#, with SQLite databases
