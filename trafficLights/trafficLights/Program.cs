using System.Collections.Immutable;

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
            public Car[] NLine = { };//Car lineup coming from the North
            public Car[] SLine = { };//Car lineup coming from the South
            public Car[] ELine = { };//Car lineup coming from the East
            public Car[] WLine = { };//Car lineup coming from the West

            public int NSScore;//Talley of Car's waitingScore from the North and South
            public int EWScore;//Talley of Car's waitingScore from the East and West

            //By Default, both lights will be Red
            public char NSLight = Globals.LIGHT_COLORS[2];
            public char EWLigth = Globals.LIGHT_COLORS[2];

        }
        static Car[] createCarList()
        {
            Random rnd = new Random();
            Car[] returnValue = { };
            for (int i = 0; i < Globals.NUM_CARS; i++)
            {
                int goingNum = rnd.Next(0, 3);
                if (goingNum == 2)
                {
                    goingNum = 0; //Ensuring Straight is the most common route; Perhaps not the most accurate...
                }
                returnValue[i] = new Car(rnd.Next(0, Globals.TIME_SECTION), Globals.DIRECTIONS[rnd.Next(0, 4)], Globals.TURNING[goingNum]);
            }
            Car[] newReturnValue = returnValue.OrderBy(x => x.arrivalTime).ToArray();
            return newReturnValue;

        }
        static bool carLogic(Intersection inter, int section)
        {
            Car carToCheck;
            switch(section){
                case 0: //North Check
                    carToCheck = inter.NLine[0];
                    if (carToCheck.going == Globals.TURNING[0]) //Straight
                    {
                        if (inter.NSLight == Globals.LIGHT_COLORS[0] || inter.NSLight == Globals.LIGHT_COLORS[1])
                        { //If Light is Green or Yellow
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (carToCheck.going == Globals.TURNING[1]) //Left
                    {
                        if (inter.NSLight == Globals.LIGHT_COLORS[0] && inter.SLine.Length == 0)
                        { //If Light is Green & Paralel Line is Empty
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    }
                    else //Right
                    {
                        if (inter.NSLight == Globals.LIGHT_COLORS[0] || inter.ELine.Length == 0)
                        { //If Light is Green or No Oncoming Traffic
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case 1: //South Check
                    break;
                case 2: //East Check
                    break;
                case 3: //West Check
                    break;
        }
    }
}


