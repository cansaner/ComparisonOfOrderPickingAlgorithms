using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class Test
    {
        private static Coordinate depot;
        private static Problem room;
        private static Parameters parameters;
        private static Picker picker;
        private static Solution solution1, solution2, solution3, solution4, solution5, solution6, solution7, solution8, solution9, solution10;

        public static void runTestCases()
        {
            int S = 7;
            double W = 2.6;
            double L = 30.4;
            double K = 2.77;
            int no_of_horizontal_aisles = 4;
            int no_of_vertical_aisles = 8;
            
            depot = new Coordinate(1, no_of_horizontal_aisles);
            room = new Problem(S, W, L, K, no_of_horizontal_aisles - 1, no_of_vertical_aisles, depot);
            parameters = new Parameters();

            picker = new Picker(depot);
            parameters.ItemListSet = Utils.readTestList("../../../files/testLists/sshape1New.txt");
            room.ItemList = parameters.ItemListSet.ElementAt(0);
            solution1 = new Solution(room, picker, parameters);
            solution1.solve(Solution.Algorithm.SShape);
            String path01 = picker.printPath();
            String travelledDistances01 = picker.printTravelledDistances();
            String pickedItems01 = picker.printPickedItems();
            double travelledTotalDistance01 = picker.printTravelledTotalDistance();

            picker = new Picker(depot);
            parameters.ItemListSet = Utils.readTestList("../../../files/testLists/sshape1New.txt");
            room.ItemList = parameters.ItemListSet.ElementAt(0);
            solution2 = new Solution(room, picker, parameters);
            solution2.solve(Solution.Algorithm.LargestGap);
            String path02 = picker.printPath();
            String travelledDistances02 = picker.printTravelledDistances();
            String pickedItems02 = picker.printPickedItems();
            double travelledTotalDistance02 = picker.printTravelledTotalDistance();

            S = 10;
            no_of_horizontal_aisles = 4;
            no_of_vertical_aisles = 12;

            room = new Problem(S, W, L, K, no_of_horizontal_aisles - 1, no_of_vertical_aisles, depot);

            picker = new Picker(depot);
            parameters.ItemListSet = Utils.readTestList("../../../files/testLists/SStestCase01.txt");
            room.ItemList = parameters.ItemListSet.ElementAt(0);
            solution3 = new Solution(room, picker, parameters);
            solution3.solve(Solution.Algorithm.SShape);
            String path03 = picker.printPath();
            String travelledDistances03 = picker.printTravelledDistances();
            String pickedItems03 = picker.printPickedItems();
            double travelledTotalDistance03 = picker.printTravelledTotalDistance();

            picker = new Picker(depot);
            parameters.ItemListSet = Utils.readTestList("../../../files/testLists/SStestCase01.txt");
            room.ItemList = parameters.ItemListSet.ElementAt(0);
            solution4 = new Solution(room, picker, parameters);
            solution4.solve(Solution.Algorithm.LargestGap);
            String path04 = picker.printPath();
            String travelledDistances04 = picker.printTravelledDistances();
            String pickedItems04 = picker.printPickedItems();
            double travelledTotalDistance04 = picker.printTravelledTotalDistance();

            picker = new Picker(depot);
            parameters.ItemListSet = Utils.readTestList("../../../files/testLists/SStestCase02.txt");
            room.ItemList = parameters.ItemListSet.ElementAt(0);
            solution5 = new Solution(room, picker, parameters);
            solution5.solve(Solution.Algorithm.SShape);
            String path05 = picker.printPath();
            String travelledDistances05 = picker.printTravelledDistances();
            String pickedItems05 = picker.printPickedItems();
            double travelledTotalDistance05 = picker.printTravelledTotalDistance();

            picker = new Picker(depot);
            parameters.ItemListSet = Utils.readTestList("../../../files/testLists/SStestCase02.txt");
            room.ItemList = parameters.ItemListSet.ElementAt(0);
            solution6 = new Solution(room, picker, parameters);
            solution6.solve(Solution.Algorithm.LargestGap);
            String path06 = picker.printPath();
            String travelledDistances06 = picker.printTravelledDistances();
            String pickedItems06 = picker.printPickedItems();
            double travelledTotalDistance06 = picker.printTravelledTotalDistance();

            picker = new Picker(depot);
            parameters.ItemListSet = Utils.readTestList("../../../files/testLists/SStestCase03.txt");
            room.ItemList = parameters.ItemListSet.ElementAt(0);
            solution7 = new Solution(room, picker, parameters);
            solution7.solve(Solution.Algorithm.SShape);
            String path07 = picker.printPath();
            String travelledDistances07 = picker.printTravelledDistances();
            String pickedItems07 = picker.printPickedItems();
            double travelledTotalDistance07 = picker.printTravelledTotalDistance();

            picker = new Picker(depot);
            parameters.ItemListSet = Utils.readTestList("../../../files/testLists/SStestCase03.txt");
            room.ItemList = parameters.ItemListSet.ElementAt(0);
            solution8 = new Solution(room, picker, parameters);
            solution8.solve(Solution.Algorithm.LargestGap);
            String path08 = picker.printPath();
            String travelledDistances08 = picker.printTravelledDistances();
            String pickedItems08 = picker.printPickedItems();
            double travelledTotalDistance08 = picker.printTravelledTotalDistance();

            picker = new Picker(depot);
            parameters.ItemListSet = Utils.readTestList("../../../files/testLists/SStestCase04.txt");
            room.ItemList = parameters.ItemListSet.ElementAt(0);
            solution9 = new Solution(room, picker, parameters);
            solution9.solve(Solution.Algorithm.SShape);
            String path09 = picker.printPath();
            String travelledDistances09 = picker.printTravelledDistances();
            String pickedItems09 = picker.printPickedItems();
            double travelledTotalDistance09 = picker.printTravelledTotalDistance();

            picker = new Picker(depot);
            parameters.ItemListSet = Utils.readTestList("../../../files/testLists/SStestCase04.txt");
            room.ItemList = parameters.ItemListSet.ElementAt(0);
            solution10 = new Solution(room, picker, parameters);
            solution10.solve(Solution.Algorithm.LargestGap);
            String path10 = picker.printPath();
            String travelledDistances10 = picker.printTravelledDistances();
            String pickedItems10 = picker.printPickedItems();
            double travelledTotalDistance10 = picker.printTravelledTotalDistance();

            String path01Correct = "(4,1) -> (4,2) -> (3,2) -> (2,2) -> (1,2) -> (1,3) -> (2,3) -> (2,4) -> (2,3) -> (3,3) -> (3,4) -> (2,4) -> (2,6) -> (3,6) -> (3,7) -> (4,7) -> (4,6) -> (3,6) -> (3,5) -> (4,5) -> (4,4) -> (3,4) -> (3,3) -> (4,3) -> (4,1)";
            String travelledDistances01Correct = "1W + 1L + 1L + 1L + 1W + 1L + 1W + 5K + 5K + 1W + 1L + 1W + 1L + 2W + 1L + 1W + 1L + 1W + 1L + 1W + 1L + 1W + 1L + 1W + 1L + 2W";
            String pickedItems01Correct = "8 -> 2 -> 3 -> 1 -> 4 -> 6 -> 5 -> 7 -> 9 -> 10 -> 12 -> 11 -> 22 -> 20 -> 21 -> 18 -> 19 -> 17 -> 15 -> 16 -> 14 -> 13";
            double travelledTotalDistance01Correct = 428.89999999999992;

            String path02Correct = "(4,1) -> (4,2) -> (3,2) -> (2,2) -> (1,2) -> (1,3) -> (1,4) -> (2,4) -> (2,3) -> (2,6) -> (3,6) -> (3,4) -> (3,3) -> (3,4) -> (3,5) -> (3,6) -> (3,7) -> (4,7) -> (4,5) -> (4,4) -> (4,3) -> (4,1)";
            String travelledDistances02Correct = "1W + 1L + 1L + 1L + 1W + 2K + 2K + 1W + 1L + 1W + 3K + 3K + 3W + 1L + 2W + 3K + 3K + 1W + 2K + 2K + 1W + 2K + 2K + 1W + 1K + 1K + 1W + 3K + 3K + 1W + 1L + 2W + 3K + 3K + 1W + 2K + 2K + 1W + 3K + 3K + 2W";
            String pickedItems02Correct = "8 -> 2 -> 3 -> 1 -> 4 -> 7 -> 5 -> 6 -> 9 -> 12 -> 11 -> 10 -> 14 -> 16 -> 18 -> 21 -> 20 -> 22 -> 17 -> 19 -> 15 -> 13";
            double travelledTotalDistance02Correct = 367.36;

            String path03Correct = "(4,1) -> (4,3) -> (3,3) -> (2,3) -> (1,3) -> (1,5) -> (2,5) -> (2,6) -> (1,6) -> (1,8) -> (2,8) -> (2,11) -> (3,11) -> (3,10) -> (2,10) -> (2,8) -> (3,8) -> (3,7) -> (2,7) -> (2,5) -> (3,5) -> (3,4) -> (4,4) -> (4,6) -> (3,6) -> (3,7) -> (4,7) -> (4,10) -> (3,10) -> (3,11) -> (4,11) -> (4,1)";
            String travelledDistances03Correct = "2W + 1L + 1L + 1L + 2W + 1L + 1W + 1L + 2W + 1L + 3W + 4K + 4K + 1L + 1W + 1L + 2W + 1L + 1W + 1L + 2W + 1L + 1W + 8K + 8K + 1L + 2W + 1L + 1W + 1L + 3W + 1L + 1W + 1L + 10W";
            String pickedItems03Correct = "1 -> 2 -> 3 -> 4 -> 6 -> 5 -> 7 -> 13 -> 12 -> 10 -> 11 -> 9 -> 8 -> 14 -> 16 -> 15 -> 18 -> 17 -> 19 -> 20 -> 21";
            double travelledTotalDistance03Correct = 641.27999999999975;

            String path04Correct = "(4,1) -> (4,3) -> (3,3) -> (2,3) -> (1,3) -> (1,5) -> (1,6) -> (1,11) -> (2,11) -> (2,8) -> (2,11) -> (2,10) -> (2,8) -> (2,5) -> (2,4) -> (3,4) -> (3,7) -> (3,6) -> (3,7) -> (3,10) -> (3,11) -> (4,11) -> (4,7) -> (4,4) -> (4,1)";
            String travelledDistances04Correct = "2W + 1L + 1L + 1L + 2W + 5K + 5K + 1W + 3K + 3K + 5W + 1L + 3W + 4K + 4K + 3W + 4K + 4K + 1W + 1K + 1K + 2W + 2K + 2K + 3W + 5K + 5K + 1W + 1L + 3W + 3K + 3K + 1W + 6K + 6K + 1W + 1K + 1K + 3W + 2K + 2K + 1W + 1L + 4W + 2K + 2K + 3W + 1K + 1K + 3W";
            String pickedItems04Correct = "1 -> 2 -> 3 -> 4 -> 7 -> 5 -> 6 -> 13 -> 12 -> 10 -> 9 -> 8 -> 11 -> 15 -> 16 -> 18 -> 20 -> 19 -> 21 -> 17 -> 14";
            double travelledTotalDistance04Correct = 507.66000000000008;

            String path05Correct = "(4,1) -> (4,2) -> (3,2) -> (2,2) -> (2,3) -> (1,3) -> (1,4) -> (2,4) -> (2,6) -> (1,6) -> (1,8) -> (2,8) -> (2,9) -> (1,9) -> (1,10) -> (2,10) -> (2,11) -> (3,11) -> (3,10) -> (2,10) -> (2,9) -> (3,9) -> (3,8) -> (2,8) -> (2,6) -> (3,6) -> (3,4) -> (2,4) -> (2,3) -> (3,3) -> (3,4) -> (4,4) -> (4,6) -> (3,6) -> (3,7) -> (4,7) -> (4,8) -> (3,8) -> (3,9) -> (4,9) -> (4,11) -> (4,1)";
            String travelledDistances05Correct = "1W + 1L + 1L + 1W + 1L + 1W + 1L + 2W + 1L + 2W + 1L + 1W + 1L + 1W + 1L + 1W + 5K + 5K + 1L + 1W + 1L + 1W + 1L + 1W + 1L + 2W + 1L + 2W + 1L + 1W + 1L + 1W + 1L + 2W + 1L + 1W + 1L + 1W + 1L + 1W + 1L + 2W + 7K + 7K + 10W";
            String pickedItems05Correct = "22 -> 21 -> 9 -> 1 -> 2 -> 3 -> 4 -> 5 -> 6 -> 7 -> 8 -> 20 -> 19 -> 18 -> 17 -> 16 -> 15 -> 13 -> 14 -> 10 -> 12 -> 11 -> 23 -> 24 -> 25 -> 26 -> 27 -> 28 -> 29 -> 30";
            double travelledTotalDistance05Correct = 768.07999999999993;

            String path06Correct = "(4,1) -> (4,2) -> (3,2) -> (2,2) -> (2,3) -> (1,3) -> (1,8) -> (1,9) -> (1,10) -> (1,11) -> (2,11) -> (2,6) -> (2,4) -> (2,3) -> (2,4) -> (2,8) -> (2,11) -> (3,11) -> (3,10) -> (3,9) -> (3,6) -> (3,4) -> (3,7) -> (3,8) -> (3,9) -> (3,11) -> (4,11) -> (4,9) -> (4,6) -> (4,1)";
            String travelledDistances06Correct = "1W + 1L + 1L + 1W + 1L + 5W + 4K + 4K + 1W + 4K + 4K + 1W + 1K + 1K + 1W + 1L + 5W + 1K + 1K + 2W + 6K + 6K + 1W + 5K + 5K + 1W + 3K + 3K + 4W + 6K + 6K + 3W + 1L + 1W + 1K + 1K + 1W + 4K + 4K + 3W + 1K + 1K + 2W + 6K + 6K + 3W + 2K + 2K + 1W + 2K + 2K + 1W + 4K + 4K + 2W + 1L + 2W + 1K + 1K + 3W + 3K + 3K + 5W";
            String pickedItems06Correct = "22 -> 21 -> 9 -> 1 -> 5 -> 6 -> 7 -> 8 -> 4 -> 3 -> 2 -> 10 -> 12 -> 11 -> 14 -> 13 -> 16 -> 17 -> 20 -> 19 -> 18 -> 15 -> 23 -> 24 -> 26 -> 27 -> 28 -> 30 -> 29 -> 25";
            double travelledTotalDistance06Correct = 611.56000000000006;

            String path07Correct = "(4,1) -> (4,2) -> (3,2) -> (2,2) -> (2,4) -> (1,4) -> (1,7) -> (2,7) -> (2,11) -> (3,11) -> (3,10) -> (2,10) -> (2,9) -> (3,9) -> (3,7) -> (2,7) -> (2,6) -> (3,6) -> (3,5) -> (3,3) -> (4,3) -> (4,4) -> (3,4) -> (3,5) -> (4,5) -> (4,6) -> (3,6) -> (3,8) -> (4,8) -> (4,11) -> (4,1)";
            String travelledDistances07Correct = "1W + 1L + 1L + 2W + 1L + 3W + 1L + 4W + 10K + 10K + 1L + 1W + 1L + 1W + 1L + 2W + 1L + 1W + 1L + 1W + 9K + 9K + 2W + 1L + 1W + 1L + 1W + 1L + 1W + 1L + 2W + 1L + 3W + 4K + 4K + 10W";
            String pickedItems07Correct = "18 -> 4 -> 1 -> 2 -> 3 -> 17 -> 16 -> 15 -> 13 -> 14 -> 11 -> 12 -> 10 -> 9 -> 8 -> 6 -> 5 -> 7 -> 19 -> 20 -> 21 -> 23 -> 24 -> 22 -> 25 -> 27 -> 26 -> 28 -> 29 -> 30";
            double travelledTotalDistance07Correct = 646.61999999999989;

            String path08Correct = "(4,1) -> (4,2) -> (3,2) -> (2,2) -> (2,4) -> (1,4) -> (1,11) -> (2,11) -> (2,7) -> (2,5) -> (2,6) -> (2,7) -> (2,10) -> (2,11) -> (3,11) -> (3,10) -> (3,9) -> (3,6) -> (3,5) -> (3,3) -> (3,6) -> (3,8) -> (3,11) -> (4,11) -> (4,8) -> (4,5) -> (4,4) -> (4,1)";
            String travelledDistances08Correct = "1W + 1L + 1L + 2W + 1L + 7W + 1L + 4W + 1K + 1K + 2W + 2K + 2K + 1W + 3K + 3K + 1W + 3K + 3K + 3W + 4K + 4K + 1W + 1L + 1W + 1K + 1K + 1W + 5K + 5K + 3W + 3K + 3K + 1W + 3K + 3K + 2W + 6K + 6K + 3W + 4K + 4K + 2W + 1K + 1K + 3W + 1L + 3W + 1K + 1K + 3W + 5K + 5K + 1W + 6K + 6K + 3W";
            String pickedItems08Correct = "18 -> 4 -> 1 -> 3 -> 2 -> 7 -> 9 -> 10 -> 14 -> 13 -> 17 -> 16 -> 15 -> 12 -> 11 -> 8 -> 6 -> 5 -> 19 -> 20 -> 21 -> 26 -> 27 -> 28 -> 30 -> 29 -> 25 -> 23 -> 24 -> 22";
            double travelledTotalDistance08Correct = 573.12000000000012;

            String path09Correct = "(4,1) -> (4,3) -> (3,3) -> (2,3) -> (3,3) -> (3,4) -> (4,4) -> (4,5) -> (3,5) -> (3,6) -> (4,6) -> (4,7) -> (3,7) -> (3,8) -> (4,8) -> (4,9) -> (3,9) -> (3,10) -> (4,10) -> (4,11) -> (4,1)";
            String travelledDistances09Correct = "2W + 1L + 1L + 10K + 10K + 1L + 1W + 1L + 1W + 1L + 1W + 1L + 1W + 1L + 1W + 1L + 1W + 1L + 1W + 1L + 1W + 7K + 7K + 10W";
            String pickedItems09Correct = "1 -> 3 -> 4 -> 5 -> 2 -> 6 -> 8 -> 7 -> 9 -> 10 -> 11 -> 13 -> 12 -> 14 -> 16 -> 17 -> 19 -> 18 -> 15 -> 20 -> 21";
            double travelledTotalDistance09Correct = 450.18000000000006;

            String path10Correct = "(4,1) -> (4,3) -> (3,3) -> (2,3) -> (3,3) -> (3,4) -> (3,5) -> (3,6) -> (3,7) -> (3,8) -> (3,9) -> (3,11) -> (4,11) -> (4,10) -> (4,7) -> (4,4) -> (4,1)";
            String travelledDistances10Correct = "2W + 1L + 1L + 10K + 10K + 1L + 1W + 4K + 4K + 1W + 5K + 5K + 1W + 4K + 4K + 1W + 2K + 2K + 1W + 4K + 4K + 1W + 6K + 6K + 2W + 1L + 1W + 2K + 2K + 3W + 4K + 4K + 3W + 1K + 1K + 3W";
            String pickedItems10Correct = "1 -> 3 -> 4 -> 5 -> 8 -> 6 -> 7 -> 9 -> 10 -> 13 -> 12 -> 14 -> 16 -> 15 -> 18 -> 19 -> 17 -> 21 -> 20 -> 11 -> 2";
            double travelledTotalDistance10Correct = 406.28000000000009;

            Console.WriteLine("\nTEST CASES:\n");
            Console.WriteLine("Solution 01 path test is " + ((String.Compare(path01, path01Correct) == 0) ? "\t\t\tpassed" : "\t\t\tfailed\n" + path01 + " SHOULD BE\n" + path01Correct));
            Console.WriteLine("Solution 01 travelled distances test is " + ((String.Compare(travelledDistances01, travelledDistances01Correct) == 0) ? "\tpassed" : "\tfailed\n" + travelledDistances01 + " SHOULD BE\n" + travelledDistances01Correct));
            Console.WriteLine("Solution 01 picked items test is " + ((String.Compare(pickedItems01, pickedItems01Correct) == 0) ? "\t\tpassed" : "\t\tfailed\n" + pickedItems01 + " SHOULD BE\n" + pickedItems01Correct));
            Console.WriteLine("Solution 01 travelled total distance test is " + ((travelledTotalDistance01 == travelledTotalDistance01Correct) ? "\tpassed\n" : "\tfailed\n" + travelledTotalDistance01 + " SHOULD BE\n" + travelledTotalDistance01Correct + "\n"));

            Console.WriteLine("Solution 02 path test is " + ((String.Compare(path02, path02Correct) == 0) ? "\t\t\tpassed" : "\t\t\tfailed\n" + path02 + " SHOULD BE\n" + path02Correct));
            Console.WriteLine("Solution 02 travelled distances test is " + ((String.Compare(travelledDistances02, travelledDistances02Correct) == 0) ? "\tpassed" : "\tfailed\n" + travelledDistances02 + " SHOULD BE\n" + travelledDistances02Correct));
            Console.WriteLine("Solution 02 picked items test is " + ((String.Compare(pickedItems02, pickedItems02Correct) == 0) ? "\t\tpassed" : "\t\tfailed\n" + pickedItems02 + " SHOULD BE\n" + pickedItems02Correct));
            Console.WriteLine("Solution 02 travelled total distance test is " + ((travelledTotalDistance02 == travelledTotalDistance02Correct) ? "\tpassed\n" : "\tfailed\n" + travelledTotalDistance02 + " SHOULD BE\n" + travelledTotalDistance02Correct + "\n"));

            Console.WriteLine("Solution 03 path test is " + ((String.Compare(path03, path03Correct) == 0) ? "\t\t\tpassed" : "\t\t\tfailed\n" + path03 + " SHOULD BE\n" + path03Correct));
            Console.WriteLine("Solution 03 travelled distances test is " + ((String.Compare(travelledDistances03, travelledDistances03Correct) == 0) ? "\tpassed" : "\tfailed\n" + travelledDistances03 + " SHOULD BE\n" + travelledDistances03Correct));
            Console.WriteLine("Solution 03 picked items test is " + ((String.Compare(pickedItems03, pickedItems03Correct) == 0) ? "\t\tpassed" : "\t\tfailed\n" + pickedItems03 + " SHOULD BE\n" + pickedItems03Correct));
            Console.WriteLine("Solution 03 travelled total distance test is " + ((travelledTotalDistance03 == travelledTotalDistance03Correct) ? "\tpassed\n" : "\tfailed\n" + travelledTotalDistance03 + " SHOULD BE\n" + travelledTotalDistance03Correct + "\n"));

            Console.WriteLine("Solution 04 path test is " + ((String.Compare(path04, path04Correct) == 0) ? "\t\t\tpassed" : "\t\t\tfailed\n" + path04 + " SHOULD BE\n" + path04Correct));
            Console.WriteLine("Solution 04 travelled distances test is " + ((String.Compare(travelledDistances04, travelledDistances04Correct) == 0) ? "\tpassed" : "\tfailed\n" + travelledDistances04 + " SHOULD BE\n" + travelledDistances04Correct));
            Console.WriteLine("Solution 04 picked items test is " + ((String.Compare(pickedItems04, pickedItems04Correct) == 0) ? "\t\tpassed" : "\t\tfailed\n" + pickedItems04 + " SHOULD BE\n" + pickedItems04Correct));
            Console.WriteLine("Solution 04 travelled total distance test is " + ((travelledTotalDistance04 == travelledTotalDistance04Correct) ? "\tpassed\n" : "\tfailed\n" + travelledTotalDistance04 + " SHOULD BE\n" + travelledTotalDistance04Correct + "\n"));

            Console.WriteLine("Solution 05 path test is " + ((String.Compare(path05, path05Correct) == 0) ? "\t\t\tpassed" : "\t\t\tfailed\n" + path05 + " SHOULD BE\n" + path05Correct));
            Console.WriteLine("Solution 05 travelled distances test is " + ((String.Compare(travelledDistances05, travelledDistances05Correct) == 0) ? "\tpassed" : "\tfailed\n" + travelledDistances05 + " SHOULD BE\n" + travelledDistances05Correct));
            Console.WriteLine("Solution 05 picked items test is " + ((String.Compare(pickedItems05, pickedItems05Correct) == 0) ? "\t\tpassed" : "\t\tfailed\n" + pickedItems05 + " SHOULD BE\n" + pickedItems05Correct));
            Console.WriteLine("Solution 05 travelled total distance test is " + ((travelledTotalDistance05 == travelledTotalDistance05Correct) ? "\tpassed\n" : "\tfailed\n" + travelledTotalDistance05 + " SHOULD BE\n" + travelledTotalDistance05Correct + "\n"));

            Console.WriteLine("Solution 06 path test is " + ((String.Compare(path06, path06Correct) == 0) ? "\t\t\tpassed" : "\t\t\tfailed\n" + path06 + " SHOULD BE\n" + path06Correct));
            Console.WriteLine("Solution 06 travelled distances test is " + ((String.Compare(travelledDistances06, travelledDistances06Correct) == 0) ? "\tpassed" : "\tfailed\n" + travelledDistances06 + " SHOULD BE\n" + travelledDistances06Correct));
            Console.WriteLine("Solution 06 picked items test is " + ((String.Compare(pickedItems06, pickedItems06Correct) == 0) ? "\t\tpassed" : "\t\tfailed\n" + pickedItems06 + " SHOULD BE\n" + pickedItems06Correct));
            Console.WriteLine("Solution 06 travelled total distance test is " + ((travelledTotalDistance06 == travelledTotalDistance06Correct) ? "\tpassed\n" : "\tfailed\n" + travelledTotalDistance06 + " SHOULD BE\n" + travelledTotalDistance06Correct + "\n"));

            Console.WriteLine("Solution 07 path test is " + ((String.Compare(path07, path07Correct) == 0) ? "\t\t\tpassed" : "\t\t\tfailed\n" + path07 + " SHOULD BE\n" + path07Correct));
            Console.WriteLine("Solution 07 travelled distances test is " + ((String.Compare(travelledDistances07, travelledDistances07Correct) == 0) ? "\tpassed" : "\tfailed\n" + travelledDistances07 + " SHOULD BE\n" + travelledDistances07Correct));
            Console.WriteLine("Solution 07 picked items test is " + ((String.Compare(pickedItems07, pickedItems07Correct) == 0) ? "\t\tpassed" : "\t\tfailed\n" + pickedItems07 + " SHOULD BE\n" + pickedItems07Correct));
            Console.WriteLine("Solution 07 travelled total distance test is " + ((travelledTotalDistance07 == travelledTotalDistance07Correct) ? "\tpassed\n" : "\tfailed\n" + travelledTotalDistance07 + " SHOULD BE\n" + travelledTotalDistance07Correct + "\n"));

            Console.WriteLine("Solution 08 path test is " + ((String.Compare(path08, path08Correct) == 0) ? "\t\t\tpassed" : "\t\t\tfailed\n" + path08 + " SHOULD BE\n" + path08Correct));
            Console.WriteLine("Solution 08 travelled distances test is " + ((String.Compare(travelledDistances08, travelledDistances08Correct) == 0) ? "\tpassed" : "\tfailed\n" + travelledDistances08 + " SHOULD BE\n" + travelledDistances08Correct));
            Console.WriteLine("Solution 08 picked items test is " + ((String.Compare(pickedItems08, pickedItems08Correct) == 0) ? "\t\tpassed" : "\t\tfailed\n" + pickedItems08 + " SHOULD BE\n" + pickedItems08Correct));
            Console.WriteLine("Solution 08 travelled total distance test is " + ((travelledTotalDistance08 == travelledTotalDistance08Correct) ? "\tpassed\n" : "\tfailed\n" + travelledTotalDistance08 + " SHOULD BE\n" + travelledTotalDistance08Correct + "\n"));

            Console.WriteLine("Solution 09 path test is " + ((String.Compare(path09, path09Correct) == 0) ? "\t\t\tpassed" : "\t\t\tfailed\n" + path09 + " SHOULD BE\n" + path09Correct));
            Console.WriteLine("Solution 09 travelled distances test is " + ((String.Compare(travelledDistances09, travelledDistances09Correct) == 0) ? "\tpassed" : "\tfailed\n" + travelledDistances09 + " SHOULD BE\n" + travelledDistances09Correct));
            Console.WriteLine("Solution 09 picked items test is " + ((String.Compare(pickedItems09, pickedItems09Correct) == 0) ? "\t\tpassed" : "\t\tfailed\n" + pickedItems09 + " SHOULD BE\n" + pickedItems09Correct));
            Console.WriteLine("Solution 09 travelled total distance test is " + ((travelledTotalDistance09 == travelledTotalDistance09Correct) ? "\tpassed\n" : "\tfailed\n" + travelledTotalDistance09 + " SHOULD BE\n" + travelledTotalDistance09Correct + "\n"));

            Console.WriteLine("Solution 10 path test is " + ((String.Compare(path10, path10Correct) == 0) ? "\t\t\tpassed" : "\t\t\tfailed\n" + path09 + " SHOULD BE\n" + path09Correct));
            Console.WriteLine("Solution 10 travelled distances test is " + ((String.Compare(travelledDistances10, travelledDistances10Correct) == 0) ? "\tpassed" : "\tfailed\n" + travelledDistances09 + " SHOULD BE\n" + travelledDistances09Correct));
            Console.WriteLine("Solution 10 picked items test is " + ((String.Compare(pickedItems10, pickedItems10Correct) == 0) ? "\t\tpassed" : "\t\tfailed\n" + pickedItems09 + " SHOULD BE\n" + pickedItems09Correct));
            Console.WriteLine("Solution 10 travelled total distance test is " + ((travelledTotalDistance10 == travelledTotalDistance10Correct) ? "\tpassed\n" : "\tfailed\n" + travelledTotalDistance09 + " SHOULD BE\n" + travelledTotalDistance09Correct + "\n"));
        }
    }
}
