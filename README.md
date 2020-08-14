# Console-Chess

This project is just a little challenge that I set for myself and is a proof of concept more than anything else as is really inconvenient to use and serves no real purpose.

## Player Input

1: Specify the piece you want to move with its corresponding letter (King = <b>K</b>, Queen = <b>Q</b>, Rook = <b>R</b>, Bishop = <b>B</b>, Knight = <b>N</b>, Pawn = <b>P</b>).  
2: Square the piece to move is currently on in the format: row (letters A-H) column (1-8). Ex.: <b>C5</b>  
3: Square to move the piece on (for format see point 2)

A possible move could look something like: <b>PE2E4</b>, <b>NB1C3</b> (Capitalization does <b>not</b> matter)

The format for castling is <b>O-O</b> for a short castle and <b>O-O-O</b> for a long castle (notice that these are <b>capital o's</b> instead of zeros)

## Special Commands

<b>dnb</b>: Redraws the board  
<b>rtb</b>: Rotates the board 180°

## Known Bugs

Resizing the window will introduce weird color bugs in the board pattern which can be resolved by entering the <b>dnb</b> command.  

Resizing the window while the board is being drawn results in even weirder background color bugs. In that case entering just the dnb command will not always work. You can solve that bug by selecting the miscolored background and entering the <b>dnb</b> command afterwards.
