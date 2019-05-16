## Compression Algorithms

Basic implementation, none of them work on binary files.

Doesn't support Windows-style newline characters (so original and decoded files may not be exactly similar).

### Naive RLE Algorithm

aaaab -> 4a1b

Impossible to decode correctly when applied to files which contain numbers.

### Huffman Algorithm

4 bytes -- amount of different chars

4 bytes -- text length

For each char: 2 bytes -- char code; 4 bytes -- char frequency.

### Improved RLE

Solves problem with files with numbers, but produces files with additional encoding information.

Doesn't work on big files (Burrows-Wheeler transform algorithm creates matrix of (amount of characters)x(amount of characters) size)

Based on Qt 5.11.2 (GCC 5.3.1 20160406 (Red Hat 5.3.1-6), 64 bit)

