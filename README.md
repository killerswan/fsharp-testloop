# F# testloop

### An example
For example, to compile a particular F# script when any F# files change,
and to run NUnit for a particular DLL if it changes, merely run the
following in an F# script:
 
```fsharp
   #load "../fsharp-testloop/Loop.fsx"
   open Loop
 
   Loop.watchFiltersAndCompileScript ["*.fs"; "*.fsx"; "*.fsi"] "CryptoTests.fsx"
   Loop.watchAndRunTestDLLs ["CryptoTests.dll"]
   Loop.run ()
```

Then go make a couple changes to a file and save it...

### Inspiration
This is inspired by, e.g.,
the test runner used by [the Clojre Koans](https://github.com/functional-koans/clojure-koans),
the [Haskell testloop](http://hackage.haskell.org/package/testloop), and
by [F# examples like this](http://natehoellein.blogspot.com/2008/04/sample-f-test-runner-and-c-too.html) (which is both more and less advanced...).

### Missing things
- The most obvious missing feature: I haven't explored directory recursion.
- A more subtle missing feature: compiling DLLs seems slower than just using `fsharpi` to call NUnit directly might be...
