# CrestronToolboxLogParser
C# program that reads in a Crestron Toolbox SIMPL Debugger Log file and filters the results.

## Synopsis

This program will read a Crestron Toolbox SIMPL Debugger log file and display it, you will also have the option to hide signals, the timestamp and other unwanted info.

## Motivation

This was originally created to help my own module to parse a Cisco SX-80 codec addressbook by taking a log of a conversation between a Cisco SX-80 and the bug ridden Crestron module for it.

I needed to look at a log file and hide all of the signals that I don't care about.

## Installation

No installation required.

## Usage

When debugging a Crestron system with SIMPL Debugger select "Logging" > "Save Current". The default log location is "~\Documents\Crestron\Toolbox\SIMPL Debugger Logs".

Open the saved file and you will see the contents, then de-select any signals you don't want to see. 


## API Reference

The code is the current documentation, feel free to create some and submit a pull request. 

## Tests

Tests are not implemented, feel free to make some.

## Contributors

Rod Driscoll: rdriscoll@avplus.net.au

## License

MIT License.
