﻿using System.Collections.Immutable;

namespace trafficLights
{
    class Globals
    {

        public static char[] DIRECTIONS = ['N', 'S', 'E', 'W'];
        public static char[] LIGHT_COLORS = [ 'G', 'Y', 'R' ];
        public static string[] TURNING = {"Straight", "Left", "Right"};
        
        //Values to mess with during testing
        public static int TIME_SECTION = 100000; //The amount of time surveyed
        public static int MINIMUM_TIME_INTERVAL = 10; //The amount of time at minimum a light will be green
        public static int NUM_CARS = 1000; //amount of cars that pass through intersection during surveyed time

    }
    class Program
    {
        class Car
        {
            public int arrivalTime; //When a car arrives at the intersection (Not exactly the light, could be the pile-up)
            public char arrivalEdge; //Which Direction the car is going when it reaches the intersection (N, S, E, W)
            public string going; //Which Direction the car will go at the intersection (Straight, Left, Right)
            public int departureTime = -1; //When a car leaves the intersection
            public double waitingScore = 1.0; //Score to talley for Intersection; should increase exponetially over time (nextScore = currentScore + (.01*pow(currentScore, 2))
            public Car(int time, char edge, string inputGoing)
            {
                arrivalTime = time;
                arrivalEdge = edge;
                going = inputGoing;
            }
        }

        class Intersection
        {
            public List<Car> NLine = new List<Car>();//Car lineup coming from the North
            public List<Car> SLine = new List<Car>();//Car lineup coming from the South
            public List<Car> ELine = new List<Car>();//Car lineup coming from the East
            public List<Car> WLine = new List<Car>();//Car lineup coming from the West

            public double NSScore;//Talley of Car's waitingScore from the North and South
            public double EWScore;//Talley of Car's waitingScore from the East and West

            //By Default, both lights will be Red
            public char NSLight = Globals.LIGHT_COLORS[2];
            public char EWLight = Globals.LIGHT_COLORS[2];

            public bool NGoing = false;
            public bool SGoing = false;
            public bool EGoing = false;
            public bool WGoing = false;

            public int response_time = 10; //lights cannot change unless response_time > Global MINIMUM_TIME_INTERVAL
        }
        static List<Car> createCarList()
        {
            Random rnd = new Random();
            List<Car> returnValue = new List<Car>();
            for (int i = 0; i < Globals.NUM_CARS; i++)
            {
                int goingNum = rnd.Next(0, 3);
                if (goingNum == 2)
                {
                    goingNum = 0; //Ensuring Straight is the most common route; Perhaps not the most accurate...
                }
                returnValue.Add(new Car(rnd.Next(0, Globals.TIME_SECTION), Globals.DIRECTIONS[rnd.Next(0, 4)], Globals.TURNING[goingNum]));
            }
            List<Car> newReturnValue = returnValue.OrderBy(x => x.arrivalTime).ToList();
            return newReturnValue;

        }
        static bool carLogic(Intersection inter, int section)
        {
            Car carToCheck;
            bool returnValue = false;
            switch (section)
            {
                case 0: //North Check
                    carToCheck = inter.NLine[0];
                    if (carToCheck.going == Globals.TURNING[0]) //Straight
                    {
                        if (inter.NSLight == Globals.LIGHT_COLORS[0] || inter.NSLight == Globals.LIGHT_COLORS[1])
                        { //If Light is Green or Yellow
                            returnValue = true;
                        }
                        else
                        {
                            returnValue = false;
                        }
                    }
                    else if (carToCheck.going == Globals.TURNING[1]) //Left
                    {
                        if (inter.NSLight == Globals.LIGHT_COLORS[0] && inter.SLine.Count == 0)
                        { //If Light is Green & Paralel Line is Empty
                            returnValue = true;
                        }
                        else
                        {
                            returnValue = false;
                        }

                    }
                    else //Right
                    {
                        if (inter.NSLight == Globals.LIGHT_COLORS[0] || inter.ELine.Count == 0)
                        { //If Light is Green or No Oncoming Traffic
                            returnValue = true;
                        }
                        else
                        {
                            returnValue = false;
                        }
                    }
                    inter.NGoing = returnValue;
                    break;
                case 1: //South Check
                    carToCheck = inter.SLine[0];
                    if (carToCheck.going == Globals.TURNING[0]) //Straight
                    {
                        if (inter.NSLight == Globals.LIGHT_COLORS[0] || inter.NSLight == Globals.LIGHT_COLORS[1])
                        { //If Light is Green or Yellow
                            returnValue = true;
                        }
                        else
                        {
                            returnValue = false;
                        }
                    }
                    else if (carToCheck.going == Globals.TURNING[1]) //Left
                    {
                        if (inter.NSLight == Globals.LIGHT_COLORS[0] && inter.NLine.Count == 0)
                        { //If Light is Green & Paralel Line is Empty
                            returnValue = true;
                        }
                        else
                        {
                            returnValue = false;
                        }

                    }
                    else //Right
                    {
                        if (inter.NSLight == Globals.LIGHT_COLORS[0] || inter.WLine.Count == 0)
                        { //If Light is Green or No Oncoming Traffic
                            returnValue = true;
                        }
                        else
                        {
                            returnValue = false;
                        }
                    }
                    inter.SGoing = returnValue;
                    break;
                case 2: //East Check
                    carToCheck = inter.ELine[0];
                    if (carToCheck.going == Globals.TURNING[0]) //Straight
                    {
                        if (inter.EWLight == Globals.LIGHT_COLORS[0] || inter.EWLight == Globals.LIGHT_COLORS[1])
                        { //If Light is Green or Yellow
                            returnValue = true;
                        }
                        else
                        {
                            returnValue = false;
                        }
                    }
                    else if (carToCheck.going == Globals.TURNING[1]) //Left
                    {
                        if (inter.EWLight == Globals.LIGHT_COLORS[0] && inter.WLine.Count == 0)
                        {//If Light is Green & Paralel Line is Empty
                            returnValue = true;
                        }
                        else
                        {
                            returnValue = false;
                        }
                    }
                    else //Right
                    {
                        if (inter.EWLight == Globals.LIGHT_COLORS[0] || inter.NLine.Count == 0)
                        { //If Light is Green or No Oncoming Traffic
                            returnValue = true;
                        }
                        else
                        {
                            returnValue = false;
                        }
                    }
                    inter.EGoing = returnValue;
                    break;
                case 3: //West Check
                    carToCheck = inter.WLine[0];
                    if (carToCheck.going == Globals.TURNING[0]) //Straight
                    {
                        if (inter.EWLight == Globals.LIGHT_COLORS[0] || inter.EWLight == Globals.LIGHT_COLORS[1])
                        { //If Light is Green or Yellow
                            returnValue = true;
                        }
                        else
                        {
                            returnValue = false;
                        }
                    }
                    else if (carToCheck.going == Globals.TURNING[1]) //Left
                    {
                        if (inter.EWLight == Globals.LIGHT_COLORS[0] && inter.ELine.Count == 0)
                        {//If Light is Green & Paralel Line is Empty
                            returnValue = true;
                        }
                        else
                        {
                            returnValue = false;
                        }
                    }
                    else //Right
                    {
                        if (inter.EWLight == Globals.LIGHT_COLORS[0] || inter.SLine.Count == 0)
                        { //If light is Green or No Oncoming Triffic
                            returnValue = true;
                        }
                        else
                        {
                            returnValue = false;
                        }
                    }
                    inter.WGoing = returnValue;
                    break;
            }
            return returnValue;
        }
        static void printInfo(int time, Intersection inter)
        {
            Console.WriteLine("Pass " + time + ":");
            Console.WriteLine("NORTH PASSAGE");
            Console.WriteLine("Car line up: " + inter.NLine.Count);
            Console.WriteLine("Score: " + inter.NSScore);
            if (inter.NLine.Count > 0)
            {
                Console.WriteLine("Direction: " + inter.NLine[0].going);
            }
            Console.WriteLine();
            Console.WriteLine("EAST PASSAGE");
            Console.WriteLine("Car line up: " + inter.ELine.Count);
            Console.WriteLine("Score: " + inter.EWScore);
            if (inter.ELine.Count > 0)
            {
                Console.WriteLine("Direction: " + inter.ELine[0].going);
            }
            Console.WriteLine();
            Console.WriteLine("SOUTH PASSAGE");
            Console.WriteLine("Car line up: " + inter.SLine.Count);
            Console.WriteLine("Score: " + inter.NSScore);
            if (inter.SLine.Count > 0)
            {
                Console.WriteLine("Direction: " + inter.SLine[0].going);
            }
            Console.WriteLine();
            Console.WriteLine("WEST PASSAGE");
            Console.WriteLine("Car line up: " + inter.WLine.Count);
            Console.WriteLine("Score: " + inter.EWScore);
            if (inter.WLine.Count > 0)
            {
                Console.WriteLine("Direction: " + inter.WLine[0].going);
            }
            Console.WriteLine();
            Console.WriteLine("North-South Light: " + inter.NSLight);
            Console.WriteLine("East-West Light: " + inter.EWLight);

        }
        static void intersection_brain(Intersection inter)
        {

        }
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Simulation...\n");
            Intersection intersection = new Intersection();
            int secCount = 0; //count for section 
            Console.WriteLine("Creating Car List...\n");
            List<Car> carList = createCarList();
            Console.WriteLine("Starting Simulation...");
            while (secCount < Globals.TIME_SECTION)
            {
                for (int i = 0; i < carList.Count; i++)//adding cars to intersection
                {
                    if (carList[i].arrivalTime == secCount)
                    {
                        if (carList[i].arrivalEdge == Globals.DIRECTIONS[0])
                        {
                            intersection.NLine.Add(carList[i]);
                        }
                        if (carList[i].arrivalEdge == Globals.DIRECTIONS[1])
                        {
                            intersection.SLine.Add(carList[i]);
                        }
                        if (carList[i].arrivalEdge == Globals.DIRECTIONS[2]) 
                        {
                            intersection.ELine.Add(carList[i]);
                        }
                        if (carList[i].arrivalEdge == Globals.DIRECTIONS[3])
                        {
                            intersection.WLine.Add(carList[i]);
                        }
                        carList.RemoveAt(i);
                        i--;
                    }
                }
                Console.Clear();
                printInfo(secCount, intersection);

                secCount++;
            }
        }
    }
}


