using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    class Program
    {
        static void Main(string[] args)
        {

            EcoAgent[] agentArray = new EcoAgent[11];

            for (int i = 0; i < agentArray.Length; i++)
            {
                agentArray[i] = new EcoAgent();

                agentArray[i].ID = i;
                agentArray[i].cash = 100;
                agentArray[i].labour = 100;
                agentArray[i].food = 100;

                if (i <= 5)
                {
                    agentArray[i].setup_agent(0);

                }
                else
                {
                    agentArray[i].setup_agent(1);
                }
                agentArray[i].global_setup(agentArray.Length);
            }


            gameLoop(agentArray);


        }


        static void gameLoop(EcoAgent[] agentArray)
        {


            //gameState(agentArray);
            for (int i = 0; i < 35; i++)
            {
                Console.WriteLine("Tick Number:" + i);
                for (int o = 0; o < agentArray.Length; o++)
                {
                  Console.WriteLine("<<<<<<<<<<<<>>>>>>>>>>>>>>");
                    Console.WriteLine("Id: " + agentArray[o].ID);
                    Console.WriteLine("cash: " + agentArray[o].cash);
                    Console.WriteLine("food: " + agentArray[o].food);
                    Console.WriteLine("Labour: " + agentArray[o].labour);
                    Console.WriteLine("Output: " + agentArray[o].output);
                    Console.WriteLine("Stockpile: " + agentArray[o].stockpile);

                }
                Console.ReadLine();

                //Update price

                for (int o = 0; o < agentArray.Length; o++)
                {
                    agentArray[o].stock_update();
                }

                for (int o = 0; o < agentArray.Length; o++)
                {
                    for (int p = 0; p < agentArray.Length; p++)
                    {
                        agentArray[o].output_update(p, agentArray[p]);
                    }
                }
/*
                for (int o = 0; o < agentArray.Length; o++)
                {
                    Console.WriteLine("----------"+ o +"---------");
                    Console.WriteLine("Type: " + agentArray[0].global_data[0, o]);
                    Console.WriteLine("Price: " + agentArray[0].global_data[1, o]);
                    Console.WriteLine("Quantity: " + agentArray[0].global_data[2, o]);

                }*/
                for (int o = 0; o < agentArray.Length; o++)
                {
                agentArray[o].buy(agentArray);
                }

                for (int o = 0; o < agentArray.Length; o++)
                {
                agentArray[o].produce();
                }






                    //Sell
                }
        }





    }


}

class EcoAgent
{

    //Agent ID
    public int ID;
    public int Type;
    int total_agents;

    //Resources
    public double cash;
    public double food;
    public double labour;

    //inputs
    // 0 = Cash // 1 = Labour // 2 = food

    public int[] inputs;
    public int output;

    public double stockpile;

    //setup agent inputs

    public void setup_agent(int agentType)
    {
        Type = agentType;
        //0 = pop // 1 = Farm

        if (agentType == 0)
        {
            //Pop
            inputs = new int[] { 2 };
            output = 1;
        }
        else
        {
            //Farm
            inputs = new int[] { 1 };
            output = 2;
        }


    }

    public void global_setup(int total_agents2)
    {
      total_agents = total_agents2;
        global_data = new double[3, total_agents];
    }

    //Add Outputs to stockpile for sale

    public void stock_update()
    {
        if (Type == 0)
        {
            //Pop
            stockpile = labour;
            labour = 0;
        }
        else
        {
            //Farm
            stockpile = food;
            food = 0;
        }
    }


    //Other Agents Data

    public double[,] global_data;
    //globaldata[output type, price per quantity, quantity available]

    public void output_update(int agent_number, EcoAgent foreign_agents)
    {
        //column 0 - OutputType
            global_data[0, agent_number] = foreign_agents.output;

        //column 1 - Price Per Quantity
            global_data[1, agent_number] =1 + GetRandomNumber(-0.04,0.05);

        //column 2 - Quantity available
            global_data[2, agent_number] = global_data[2, agent_number] + foreign_agents.stockpile;

    }

//used for variables like profit margin
    public double GetRandomNumber(double minimum, double maximum)
    {
        Random random = new Random();
        return random.NextDouble() * (maximum - minimum) + minimum;
    }

    public int find_cheapest()
    {
      double cheapest_price=999999;
      int cheapest=-1;

      int current_output;
      for(int i = 0; i<total_agents;i++)
      {
        current_output=(int)global_data[0,i];
        if( inputs.Contains(current_output))
        {
          if(global_data[2,i]>0){
            if(global_data[1,i]<cheapest_price)
            {
              cheapest=i;
              cheapest_price=global_data[1,i];
            }
          }
        }
      }
      return cheapest;
    }

//Implement paying
    public void buy(EcoAgent[] agentArray)
    {
      int current_available_cheapest = find_cheapest();
      bool loop = true;
      while(cash>=global_data[1, current_available_cheapest] && loop==true)
      {

        double total_value_sale = global_data[1,current_available_cheapest] * global_data[2,current_available_cheapest];
        double remainder = total_value_sale - cash;

        if(remainder <=0){
          
          if (Type == 0)
          {
              //Pop
              labour = global_data[2,current_available_cheapest];
              agentArray[current_available_cheapest].labour = 0;
          }
          else
          {
              //Farm
              food = global_data[2,current_available_cheapest];
              agentArray[current_available_cheapest].food = 0;
          }
          global_data[2,current_available_cheapest]=0;
          agentArray[current_available_cheapest].cash = agentArray[current_available_cheapest].cash + total_value_sale;
                cash = cash - total_value_sale;


            } else
        {
          if (Type == 0)
          {
              //Pop
              labour = (global_data[1,current_available_cheapest] * cash );
              agentArray[current_available_cheapest].labour = agentArray[current_available_cheapest].labour - (global_data[1,current_available_cheapest] * cash );
          }
          else
          {
              //Farm
              food = (global_data[1,current_available_cheapest] * cash );
              agentArray[current_available_cheapest].food = agentArray[current_available_cheapest].food - (global_data[1,current_available_cheapest] * cash );

          }
          global_data[2,current_available_cheapest]= global_data[2,current_available_cheapest] - (global_data[1,current_available_cheapest] * cash );
          agentArray[current_available_cheapest].cash = agentArray[current_available_cheapest].cash + total_value_sale;
          cash = cash - total_value_sale;
        }

int old_cheapest = current_available_cheapest;
  current_available_cheapest = find_cheapest();
  if(current_available_cheapest == old_cheapest){
    loop = false;
  }

            if (current_available_cheapest == -1)
            {
                current_available_cheapest = 0;
                loop = false;
            }


      }

    }

    public void produce()
    {

      if (Type == 0)
      {
          //Pop
          labour = stockpile;
          food = 0;
      }
      else
      {
          //Farm
          food = stockpile;
          labour = 0;
      }

    }



}
