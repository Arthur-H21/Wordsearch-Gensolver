# Square word search Generator and Solver

This Console Application is able to generate and solve any square word search which has been placed in a `.txt` file.

## Command Line Arguments

The `wordsearch_gensolve.exe` application can be called in the command line with the following arguments

    Command     Functions                                       Syntax 
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    generate    Generates a square problem and solution         generate `size` `wordlist filename`
                with a given size and wordlist file
    solve       Solves a square problem with a correctly        solve `problem filename`
                formatted given lettermap and wordlist file
    help        Help on the commands of the application         help

An example of the `generate` function: `./wordsearch_gensolve generate 20 wordlist1`. This will generate a 20x20 lettermap problem file and solution file from the words placed in `wordlist1.txt`.

An example of the `solve` function: `./wordsearch_gensolve solve wordsearch1`. This will solve any square problem from the file `wordsearch1.txt` and output the solution.

## Input File Formatting

**Generate**: Place all words on the first line of a `.txt` file, each separated by a space. See `wordlist1.txt`

**Solve**: Place the lettermap first in the file (letters can be separated or not separated by a space). On the last line of the file, place the words to be found on a single line, in the same way you would for a `generate` input file. See `wordserach1.txt`

## Output File Formatting

**Generate**: The problem file, with the `_wordsearch.txt` appendage, should like exactly like an input `solve` file. The solution file, with the `_wordsearch_solution.txt` appendage, should contain the same lettermap produced, but with blanks where the words are located.

**Solve**: The solution file, with the `_solution.txt` appendage, should contain same lettermap as inputted, but with blanks where the words are located. If not all words could be found (or there is an error in your input file), a file with the `_log.txt` will be produced with a solution lettermap containing solved words, and a list of words that could not be found

## Future improvements

The `generate` function needs to be able to self detect cases where the given size argument is too small. At the moment, it will run infinitely for such case.