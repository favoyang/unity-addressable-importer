# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.2.1] - 2019-08-04
 - Added address/group replacement. Working with regex rule to build the address based on information captured from the path.
 - Added path elements extraction for address/group replacement (i.e. `${PATH[0]}`)
 - Added option to automatically create groups if not exist, default False.
 - Added option to remove empty Asset Groups except the default group, default False.
 - Added option to define if labels from ruleset are added or replace the current ones.
 - Improved documentation.

## [0.1.1] - 2019-07-19
 - Updated to addressable v1.1.5.

## [0.1.0-preview] - 2019-06-30
 - Initial submission for package distribution.
