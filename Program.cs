using System;
using System.Collections.Generic;
using System.Threading;

namespace SharpMiner
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Welcome to Moon Miner!");
      new MinerGame();
    }
  }

  // Upgrade blueprint
  class Upgrade
  {
    public string Name { get; private set; }
    public string Type { get; private set; }
    public int Price { get; set; }

    public int Quantity { get; set; }
    public int GenValue { get; private set; }
    public Upgrade(string name, string type, int price, int quantity, int genValue)
    {
      Name = name;
      Type = type;
      Price = price;
      Quantity = quantity;
      GenValue = genValue;
    }
  }


  // Creation of game is what runs game.
  class MinerGame
  {
    public bool Running { get; set; }
    public bool inShop { get; set; }
    public int Cheese { get; private set; }
    public List<Upgrade> Shop { get; private set; }
    public List<Upgrade> ClickUpgrades { get; private set; }
    public List<Upgrade> AutoUpgrades { get; private set; }

    public Dictionary<string, int> Stats { get; private set; }
    public MinerGame()
    {
      Cheese = 0;
      Shop = new List<Upgrade>() { };
      ClickUpgrades = new List<Upgrade>() { new Upgrade("Pick Axe", "click", 0, 1, 1) };
      AutoUpgrades = new List<Upgrade>() { };
      Stats = new Dictionary<string, int>();
      Running = true;
      Stats.Add("Pick Axe", 1);
      Shop.Add(new Upgrade("Pick Axe", "click", 25, 0, 1));
      Shop.Add(new Upgrade("Cheese Drill", "click", 50, 0, 5));
      Shop.Add(new Upgrade("Mousetronaut", "auto", 100, 0, 1));
      Shop.Add(new Upgrade("Cheese Refiner", "auto", 1000, 0, 10));
      PlayGame();
    }

    private void PlayGame()
    {

      startTimer();
      while (Running)
      {
        ConsoleKeyInfo input = GetUserKey();
        switch (input.Key.ToString().ToLower())
        {
          case "spacebar":
            Mine();
            break;

          case "tab":
            PeruseShop();
            break;
          case "esc":
            Running = false;
            break;

        }
      }
    }
    private void Mine()
    {
      ClickUpgrades.ForEach(miner =>
      {
        Cheese += miner.GenValue * miner.Quantity;
      });
    }

    // Mines but on auto upgrades
    public void AutoMine(object o)
    {
      AutoUpgrades.ForEach(miner =>
     {
       Cheese += (miner.GenValue * miner.Quantity) * 5;
     });
      //  Keeps screen from Re-drawing if in shop
      if (inShop == false)
      {
        DrawGameScreen();
      }
    }


    // Automatic Cheese Interval
    private void startTimer()
    {
      Timer timer = new Timer(AutoMine, null, 0, 5000);
    }



    // opens shop
    private void PeruseShop()
    {
      inShop = true;
      Console.Clear();
      Console.WriteLine("Welcome to the interstellar shop, what would you like to buy?");
      for (int i = 0; i < Shop.Count; i++)
      {
        Upgrade item = Shop[i];
        Console.WriteLine($"{i + 1}.  {item.Name}: ${item.Price}, Generates- {item.GenValue} Cheese");
      }
      string choice = Console.ReadLine();
      if (int.TryParse(choice, out int selection) && selection > 0 && selection <= Shop.Count)
      {
        BuyUpgrade(Shop[selection - 1]);
      }
    }

    private void BuyUpgrade(Upgrade item)
    {
      if (Cheese >= item.Price)
      {
        Cheese -= item.Price;
        item.Price += item.Price;
        if (item.Type == "click")
        {
          int index = ClickUpgrades.FindIndex(i => i.Name == item.Name);
          if (index == -1)
          {
            ClickUpgrades.Add(item);
            index = ClickUpgrades.Count - 1;
          }
          ClickUpgrades[index].Quantity++;
          Stats[item.Name] = ClickUpgrades[index].Quantity;
        }
        else
        {
          int index = AutoUpgrades.FindIndex(i => i.Name == item.Name);
          if (index == -1)
          {
            AutoUpgrades.Add(item);
            index = AutoUpgrades.Count - 1;
          }
          AutoUpgrades[index].Quantity++;
          Stats[item.Name] = AutoUpgrades[index].Quantity;
        }
        Console.Beep();
        Console.WriteLine($"Purchased 1 {item.Name}, you now have {Stats[item.Name]}");
      }
      else
      {
        Console.WriteLine($"You do not have enough cheese for {item.Name}");
      }
      Console.WriteLine("press any key to continue");
      inShop = false;
      Console.ReadKey();
    }
    private ConsoleKeyInfo GetUserKey()
    {
      DrawGameScreen();
      return Console.ReadKey();
    }


    private void DrawGameScreen()
    {
      Console.Clear();
      Console.ForegroundColor = ConsoleColor.DarkYellow;
      string moon = $@"
                                                                                      
                              %%&&&&&&%                                         
                       %%%%%%%&%%%%&&&&&&&&&&&                                  
                   (###(((###%###%%&%&&%&%###(%&&%                              
                 ,***///(####((///(%%%##%%%&&%&&&%&%                            
                 .,*,*/(/#(/*//((/(///(///(%#&(((#%&&&                          
                  ..,,*/////**////////(///(((#(#((#%&&&&                        
                  ....,*/**(/**/*/#(**////(%#%%##(#%%&%&&                       
                    ..,.,,#/***,*/(##%(((%(##%%%%%%%%&&&&&                      
                     ...,,,,,*//((####(#&%%%&%###(%%&&&&&&&                     
                      ...,,..,/*/(((####%#%%%%%&#%%%&&@@&&&                     
                        .....,**////#((##%#%%%%&%&%&&&&&&&%                     
                         ....,,**////((((##%##&%%%%%&&&%%&%                     
                            .....,***//#((###%&%#%%%%&&%&%%                     
                              ......**((#######%#%%%%%%%&%                      
                                .....,*(#(#(##%((#%%%%%&%                       
                                  ...,**/##/(#%###%##%%%                        
                                      * * /#(((((% #%#                          
                                         ..*,@* /(%&                            
                                                                                
      ";
      Console.WriteLine(moon);
      string message = "";
      message += $@"
      Mine [Space],  Shop [tab], Quit[esc]
      Cheese: {Cheese}
      Stats:";
      foreach (var stat in Stats)
      {
        message += $"\n       {stat.Key} : {stat.Value}";
      }
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.WriteLine(message);
    }
  }



}
