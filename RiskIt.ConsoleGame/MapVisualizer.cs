using RiskIt.ConsoleGame.Models;
using RiskIt.Main.Models;

namespace RiskIt.ConsoleGame
{
    public class MapVisualizer
    {
        public static ConsoleColor[] Colors = new ConsoleColor[]
        {
            ConsoleColor.DarkRed,
            ConsoleColor.DarkBlue,
        };

        public static LinkedList<PaintArea>[] PrintMap(List<Area<string>> gameMap, (int y, int x)[] mapAreas)
        {
            // all items are 2dim x 2dim (aka a square)
            // in the grid where top left corner is 0,0 going positive down and right
            // the figures create a 5 sided star
            int dim = Program.MAP_VISUALIZE_DIM;


            // yes, I checked that this corresponds with the connections
            // and the order in the map
            int MAX_LENGTH = dim * 6;

            LinkedList<PaintArea>[] lines = CreateEmptyGrid(MAX_LENGTH);

            for (int mapAreaIndex = 0; mapAreaIndex < mapAreas.Length; mapAreaIndex++)
            {
                var count = 2 * dim;

                (int y, int x) mapArea = mapAreas[mapAreaIndex];
                Area<string> gameMapArea = gameMap
                    .FirstOrDefault(area => area.Id == mapAreaIndex.ToString());

                PaintArea pa = new PaintArea
                {
                    Foreground = Colors[gameMapArea.Player.Id],
                    Char = 'X',
                    Length = 2 * dim,
                    Area = gameMapArea
                };

                for (int lineY = mapArea.y; lineY < mapArea.y + (2 * dim); lineY++)
                {
                    LinkedList<PaintArea> currLine = lines[lineY];

                    int acc = 0;
                    LinkedListNode<PaintArea>? node = currLine.First;

                    while (node is not null)
                    {
                        var lineItem = node.Value;

                        if ((lineItem.Length + acc) > mapArea.x)
                        {
                            PaintArea newPA = new PaintArea
                            {
                                Background = pa.Background,
                                Foreground = pa.Foreground,
                                Char = pa.Char,
                                Length = pa.Length,
                                Area = pa.Area
                            };

                            // diff between where we are now (acc)
                            // and the start of the figure
                            var newNode = InsertNewItem(node: ref node,
                                          currItemConfig: lineItem,
                                          startPosCurrItem: acc,
                                          startPosNewItem: mapArea.x,
                                          newItem: newPA);


                            // Create an empty box in the middle
                            if (lineY > mapArea.y && ((mapArea.y + (2 * dim) - 1) > lineY))
                                newNode = InsertItemInto(node: ref newNode);

                            // first empty line from the top
                            if (lineY == mapArea.y + 1)
                            {
                                InsertItemInto(node: ref newNode,
                                               input: $"Id: {node.Value.Area.Id}",
                                               margin: 0);
                            }

                            // 3rd line for troops
                            if (lineY == mapArea.y + 3)
                            {
                                InsertItemInto(node: ref newNode,
                                               input: $"{node.Value.Area.Troops}",
                                               margin: 0);
                            }


                            break;
                        }
                        acc += lineItem.Length;
                        node = node.Next;
                    }
                }
            }
            return lines;
        }

        private static LinkedList<PaintArea>[] CreateEmptyGrid(int MAX_LENGTH)
        {
            LinkedList<PaintArea>[] lines = new LinkedList<PaintArea>[MAX_LENGTH];

            for (int i = 0; i < lines.Length; i++)
            {
                PaintArea empty = new PaintArea
                {
                    Background = ConsoleColor.Black,
                    Foreground = ConsoleColor.White,
                    Char = ' ',
                    Length = MAX_LENGTH
                };
                lines[i] = new LinkedList<PaintArea>();
                lines[i].AddFirst(empty);
            }

            return lines;
        }

        private static LinkedListNode<PaintArea> InsertItemInto(ref LinkedListNode<PaintArea> node,
                                                                string input = "",
                                                                int margin = 1)
        {
            PaintArea item = node.Value;

            int maxLength = item.Length;

            int textMaxLength = maxLength - (2 * margin);


            PaintArea newDuplicateItem;

            if (input == "")
            {
                item.Length = margin;
                newDuplicateItem = CreateDuplicatePA(item);

                PaintArea newEmpty = new PaintArea
                {
                    Background = ConsoleColor.Black,
                    Foreground = ConsoleColor.White,
                    Char = ' ',
                    Length = textMaxLength
                };


                node.List?.AddAfter(node, newDuplicateItem);
                return node.List?.AddAfter(node, newEmpty);
            }

            if (textMaxLength < input.Length)
                throw new Exception("Word too long");

            // add padding, prefer right pad
            // calculate padding amount
            int totalPad = textMaxLength - input.Length;

            int rightPad = totalPad / 2;
            int leftPad = totalPad - rightPad;

            List<PaintArea> paintAreas = new List<PaintArea>();

            PaintArea leftPadding = new PaintArea
            {
                Background = ConsoleColor.Black,
                Foreground = ConsoleColor.White,
                Char = ' ',
                Length = leftPad,
            };
            paintAreas.Add(leftPadding);


            PaintArea inputWord = new PaintArea
            {
                Background = ConsoleColor.Black,
                Foreground = ConsoleColor.White,
                Word = input,
            };
            paintAreas.Add(inputWord);

            PaintArea rightPadding = new PaintArea
            {
                Background = ConsoleColor.Black,
                Foreground = ConsoleColor.White,
                Char = ' ',
                Length = rightPad,
            };
            paintAreas.Add(rightPadding);


            item.Length = margin;

            newDuplicateItem = CreateDuplicatePA(item);

            if (newDuplicateItem.Length != 0)
                node.List?.AddAfter(node, newDuplicateItem);



            node.List?.AddAfter(node, rightPadding);
            var res = node.List?.AddAfter(node, inputWord);
            node.List?.AddAfter(node, leftPadding);
            if (item.Length == 0)
                node.List.Remove(node);
            node = res;
            return res;

        }



        private static PaintArea CreateDuplicatePA(PaintArea item)
        {
            return new PaintArea
            {
                Background = item.Background,
                Foreground = item.Foreground,
                Char = item.Char,
                Length = item.Length
            };
        }

        private static LinkedListNode<PaintArea> InsertNewItem(ref LinkedListNode<PaintArea> node,
                                                               PaintArea currItemConfig,
                                                               int startPosCurrItem,
                                                               int startPosNewItem,
                                                               PaintArea newItem)
        {
            int newPrevItemLength = startPosNewItem - startPosCurrItem;


            PaintArea newEmpty = new PaintArea
            {
                Background = currItemConfig.Background,
                Foreground = currItemConfig.Foreground,
                Char = currItemConfig.Char,
                Length = currItemConfig.Length - newPrevItemLength - newItem.Length
            };





            currItemConfig.Length = newPrevItemLength;

            if (newEmpty.Length != 0)
            {
                node.List?.AddAfter(node, newEmpty);
            }
            var ret = node.List?.AddAfter(node, newItem);


            if (newPrevItemLength == 0)
            {
                node.List.Remove(node);
            }
            node = ret;

            return ret;
        }

    }
}
