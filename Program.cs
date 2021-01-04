using System;
using System.Collections.Generic;

namespace NDRPS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Nondeterministic Rock Paper Scissors";
            
            //probabilities and number of games
            Random random = new Random();
            double pcProbRock = 1/3; //initial probabilities
            double pcProbPaper = 1/3;
            double pcProbScissor = 1/3;
            double playerProbRock;
            double playerProbPaper;
            double playerProbScissor;
            int maxScore; //play to this points
            int simNum; //number of simulations in a row
            int playerScore = 0;
            int fa0, fa1, fa2; //frequencies of player actions
            int fc0, fc1, fc2; //frequencies of computer actions
            fa0 = fa1 = fa2 = fc0 = fc1 = fc2 = 0;
            int hard = 0;

            Console.WriteLine("Welcome to Rock-Paper-Scissors.");
            Console.WriteLine("You get 1 point for win and taken 1 point for losing.");
            
            hard = ChooseDif(); //0 - easy, 1 - hard, 2 - impossible

            if(hard == 0) //randomize computer probs for easy mode
            {
                pcProbRock = random.NextDouble();
                pcProbPaper = random.NextDouble();
                pcProbScissor = random.NextDouble();
                double aux = pcProbRock + pcProbPaper + pcProbScissor; //normalize
                pcProbRock = pcProbRock/aux;
                pcProbPaper = pcProbPaper/aux;
                pcProbScissor = pcProbScissor/aux;
            }
            
            maxScore = ChooseMax(); //choose max number of points

            string input;
            int playerAction; //0-rock, 1-paper, 2-scissors
            int pcAction; //the same
            double plaction; //needed to simulate player action according to probs
            double inf = pcProbRock;
            double sup = 1 - pcProbScissor;
            
            bool gameOver = false;
            
            while(!gameOver)
            {
                Console.Write("Probability of Rocks (%): ");
                input = Console.ReadLine();
                bool sucess = double.TryParse(input, out playerProbRock);
                while(!sucess || playerProbRock < 0 || playerProbRock > 100) //making it idiot proof (MIIP) - see if input is between 0 and 100
                {
                    Console.WriteLine("Not valid, choose a probability between 0 and 100: ");
                    input = Console.ReadLine();
                    sucess = double.TryParse(input, out playerProbRock);
                }
            
                Console.Write("Probability of Paper (%): ");
                input = Console.ReadLine();
                sucess = double.TryParse(input, out playerProbPaper);
                while(!sucess || playerProbPaper < 0 || playerProbPaper > 100-playerProbRock) //MIIP - see if sum does not go over 100
                {
                    Console.WriteLine("Not valid, make sure the probabilities do not go over 100. Choose probability: "); 
                    input = Console.ReadLine();
                    sucess = double.TryParse(input, out playerProbPaper);
                }

                playerProbRock = playerProbRock/100; //normalize
                playerProbPaper = playerProbPaper/100;
                playerProbScissor = 1-playerProbPaper-playerProbRock;

                Console.Write("Number of simulations: "); //choosing number of simulations
                input = Console.ReadLine();
                sucess = int.TryParse(input, out simNum);

                while(!sucess || simNum < 1) 
                {
                    Console.WriteLine("Input not valid, must be an integer greater than 0: "); //MIIP - we need at least 1 simulation
                    input = Console.ReadLine();
                    sucess = int.TryParse(input, out simNum);
                }

                int index = 0; //index for simulations

                while (index < simNum && playerScore < maxScore && playerScore > -maxScore)
                {
                    pcAction = CompChooseAction(pcProbRock, pcProbPaper, fa0,fa1,fa2,hard); //simulating computer action

                    switch(pcAction)
                    {
                        case 0:
                            fc0 += 1;
                        break;
                        
                        case 1:
                            fc1 += 1;
                        break;

                        default:
                            fc2 += 1;
                        break;
                    }

                    plaction = random.NextDouble(); //simulating player action
                    if(plaction <= playerProbRock)
                    {
                        playerAction = 0;
                        fa0 += 1;
                    }
                    else if(plaction >= playerProbRock+playerProbPaper)
                    {
                        playerAction = 2;
                        fa2 += 1;
                    }
                    else{
                        playerAction = 1;
                        fa1 += 1;
                    }

                    int result = Simul(playerAction,pcAction); //round
                    playerScore += result;

                    Console.WriteLine("Player Score: " + playerScore + ", Computer Score: " + -playerScore);
                    
                    index ++; //increase simulation index
                }

                Console.WriteLine("Computer has played Rock " + fc0 + " times, Paper " + fc1 + " times, and Scissors " + fc2 + " times.");

                string playAgain = "y"; //see if player wants to play again

                if(playerScore == maxScore)
                {
                    
                    Console.WriteLine("Congratulations, you won! Type y to play again: ");
                    playAgain = Console.ReadLine();
                }
                else if(playerScore == -maxScore)
                {
                    Console.WriteLine("Too bad, you lost! Type y to play again: ");
                    playAgain = Console.ReadLine();
                }

                gameOver = playAgain != "y";

                if(gameOver == false && (playerScore == maxScore || playerScore == - maxScore))
                {
                    fa0 = fa1 = fa2 = 0; //reset player action frequencies
                    fc0 = fc1 = fc2 = 0; //reset computer action frequencies
                    playerScore = 0; //reset score
                    hard = ChooseDif(); //choose the difficulty
                    maxScore = ChooseMax(); //choose the max number of points
                }
            }
                    
            
            Console.ReadKey();
        }


        public static int CompChooseAction(double pcRock, double pcPaper, int int0, int int1, int int2, int hardness) 
        //escolhe a acção com base nas escolhas do jogador
        //o input são as frequências das escolhas do jogador
        //o output é a escolha do computador 0-pedra, 1-papel, 2-tesoura
        {
            int diff = hardness;
            double rand = 0;
            int action;
            int sum = int0 + int1 + int2; //total number of simulations done
            Random random = new Random();
            action = random.Next(1,sum + 1); //choose action based on previous player actions
            
            if (sum !=0) //in case it is not the first round of the game
            {
                switch(diff)
                {
                    case 0:
                    {
                        rand = random.NextDouble();
                        if(rand <= pcRock)
                        {
                            action = 0;
                        }
                        else if (rand >= pcRock + pcPaper)
                        {
                            action = 2;
                        }
                        else
                        {
                            action = 1;
                        }
                    }
                    break;

                    case 1:
                    {
                        if(int0 > int1 && int0 > int2)
                        {
                            action = 1;
                        }
                        else if(int1 > int0 && int1 > int2)
                        {
                            action = 2;
                        }
                        else if(int2 > int0 && int2 > int1)
                        {
                            action = 0;
                        }
                        else if(int0 == int1 && int0 > int2)
                        {
                            action = random.Next(1,3);
                        }
                        else if(int0 == int2 && int0 > int1)
                        {
                            action = random.Next(0,2);
                        }
                        else if(int1 == int2 && int1 > int0)
                        {
                            action = random.Next(0,2)*2;
                        }
                        else
                        {
                            action = random.Next(0,3);
                        }
                    }
                    break;
                    
                    case 2:
                    {
                        if(action <= int0)
                        {
                            action = 1;
                        }
                        if(action > int0 + int1)
                        {
                            action = 0;
                        }
                        else
                        {
                            action = 2;    
                        }
                    }
                    break;

                    default:
                    break;
                }
            }
            else //in the first round the pc action is always uniformly distributed
            {
                action = random.Next(0,3);
            }
            return action;
        }
        public static int ChooseMax() //choosing max number of points
        {
            Console.Write("Choose maximum points (5, 10 or 15): ");
            string input = Console.ReadLine();

            while(input != "5" && input != "10" && input!= "15") //MIIP
            {
                Console.WriteLine("Input not valid, choose between 5, 10 or 15: ");
                input = Console.ReadLine();
            }

            int max = Convert.ToInt32(input);

            return max;
        }

        public static int ChooseDif() //choosing dificulty
        // 0 - easy, 1-hard, 2 - impossible
        {
            int hard = 0;
            Console.WriteLine("Choose difficulty:");
            Console.WriteLine("0 - easy");
            Console.WriteLine("1 - hard");
            Console.WriteLine("2 - so impossible it is not even funny");
            string input = Console.ReadLine();
            
            while(input != "0" && input != "1" && input != "2") //MIIP
            {
                Console.WriteLine("Please type 0, 1 or 2: ");
                input = Console.ReadLine();
            }

            hard = Convert.ToInt32(input);
            return hard;
        }

        public static int Simul(int actPlayer, int actPc) //simulation given both actions
        //0-rock 1-paper 2-scissors
        {
            int result=0; //1 if player wins, -1 if player loses, 0 if draw
            switch (actPlayer)
            {
                case 0:
                
                    switch(actPc)
                    {
                        case 0:
                        Console.WriteLine("You choose rock, computer choose rock. Draw!");
                        result = 0;
                        break;

                        case 1:
                        Console.WriteLine("You choose rock, computer choose paper. You lost!");
                        result = -1;
                        break;

                        case 2:
                        Console.WriteLine("You choose rock, computer choose scissors. You won!");
                        result = 1;
                        break;

                        default:
                        break;
                    }
                
                break;

                case 1:
                
                    switch(actPc)
                    {
                        case 0:
                        Console.WriteLine("You choose paper, computer choose rock. You won!");
                        result = 1;
                        break;

                        case 1:
                        Console.WriteLine("You choose paper, computer choose paper. Draw!");
                        result = 0;
                        break;

                        case 2:
                        Console.WriteLine("You choose paper, computer choose scissors. You lost!");
                        result = -1;
                        break;

                        default:
                        break;
                    }
                
                break;

                case 2:
                
                
                    switch(actPc)
                    {
                        case 0:
                        Console.WriteLine("You choose scissor, computer choose rock. You lost!");
                        result = -1;
                        break;

                        case 1:
                        Console.WriteLine("You choose scissor, computer choose paper. You won!");
                        result = 1;
                        break;

                        case 2:
                        Console.WriteLine("You choose scissor, computer choose scissor. Draw!");
                        result = 0;
                        break;

                        default:
                        break;
                    }
                break;
                

                default:
                break;   
                }

            return result;

        }
    }
}

/*a fazer
- mais 1 ou 2 jogos, com outras opções e outros payouts (talvez)
- nesse caso, ou generalizar o código (não sei se dá), ou fazer código análogo para esses jogos
*/