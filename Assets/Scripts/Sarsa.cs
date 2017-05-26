using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class Sarsa : MonoBehaviour
{
    //[Header('')]
    public InputField alpha;
    public InputField gamma;
    public InputField egreedy;
    public InputField episodes;
    public InputField goalReward;
    public InputField trapReward;
    public InputField simpleReward;

    public InputField afterEpisodes1;
    public InputField afterEGreddy1;

    //public Text[] Qtable;
    public string allQ; 
    
    //Show Data
    public Text SpeedText;
    public Text EpisodesCarriedOut;
    public Text EpisodesLeft;
    public Text Moves;
    public Text eGreddyText;

    public MoveMouse mouse;
    

    public State[,] state;
    

    private int countEpisodes = 0;
    private int countMoves = 0;
    private State State1;
    private State State2;
    private string Action1;
    private string Action2;
    float QValue;
    float qValue;

    private float Speed;
    

    private float _alpha;
    private float _gamma;
    private float _eGreddy;
    private int _episodes;
    private int _afterEpisodes1;
    private float _afterEGreddy1;
    private int _goalReward;
    private int _trapReward;
    private int _simpleReward;

    public int Episodes { get { return _episodes; } set { if ((value > 0)) _episodes = value; } }
    public float Alpha { get { return _alpha; } set { if ((value > 0)) _alpha = value; } }
    public float Gamma { get { return _gamma; } set { if ((value > 0)) _gamma = value; } }
    public float eGreddy { get { return _eGreddy; } set { if ((value > 0)) _eGreddy = value; } }
    public int AfterEpisodes1 { get { return _afterEpisodes1; } set { if ((value > 0)) _afterEpisodes1 = value; } }
    public float AfterEGreddy1 { get { return _afterEGreddy1; } set { if ((value > 0)) _afterEGreddy1 = value; } }
    public int GoalReward { get { return _goalReward; } set { _goalReward= value; } }
    public int TrapReward { get { return _trapReward; } set {_trapReward = value; } }
    public int SimpleReward { get { return _simpleReward; } set { _simpleReward= value; } }

    //coordinates
    public static int Xaxis;
    public static int Yaxis;
    public static int NextXaxis;
    public static int NextYaxis;
    int minAxis = 0;
    int maxAxis = 3;

   
    void Start()
    {
        
        // Time foe eatch Move
        Speed = 1;
        ShowSpeed(Speed);

        // Default Learning settings
        Alpha = .1f;
        Gamma = .9f;
        eGreddy = 3;
        Episodes = 700;
        AfterEpisodes1 = 300;
        AfterEGreddy1 = 2;

        //Default Rewards
        GoalReward = 20;
        TrapReward = -10;
        SimpleReward = -1;
        
        //The First State
        Xaxis = 0;
        Yaxis = 0;
        NextXaxis = 0;
        NextYaxis = 0;
        
        }
    
    public void StartLearning()
    {
        // Define Rewards
        if (!(simpleReward.text == ""))
            SimpleReward = int.Parse(simpleReward.text);
        if (!(goalReward.text == ""))
            GoalReward = int.Parse(goalReward.text);
        if (!(trapReward.text == ""))
            TrapReward = int.Parse(trapReward.text);


        // Define States
        state = new State[4, 4];
        int _Qnumber = 0;

        for (int y = 0; y < 4; y++)
        {
            for (int i = 0; i < 4; i++)
            {
                state[i, y] = new State();
                state[i, y].Qnumber = _Qnumber;
                _Qnumber++;

                if ((i == 2 & y == 1) || (i == 1 & y == 3))
                {
                    state[i, y].Reward = TrapReward;
                }
                else if (i == 3 & y == 2)
                {
                    state[i, y].Reward = GoalReward;
                }
                else
                {
                    state[i, y].Reward = SimpleReward;
                }

            }
        }


        //Define Learning Settings
        if (!(alpha.text == ""))   
            Alpha = float.Parse(alpha.text);
        if (!(gamma.text == "")) 
            Gamma = float.Parse(gamma.text);
        if (!(egreedy.text == ""))
            eGreddy = float.Parse(egreedy.text);
        if (!(episodes.text == ""))
            Episodes = int.Parse(episodes.text);
        if (!(afterEpisodes1.text == ""))
            AfterEpisodes1 = int.Parse(afterEpisodes1.text);
        if (!(afterEGreddy1.text == ""))
            AfterEGreddy1 = int.Parse(afterEGreddy1.text);

        print(GoalReward + "  " + TrapReward + "  " + SimpleReward);
        StartCoroutine(Learning());
    }


    IEnumerator Learning()
    {
        //this Run only the first time
        
        //******///
        //Choose the very first move of the mouse
       
        State1 = state[Xaxis, Yaxis];
        Action1 = "Up";
        print("First Action" + Action1);
        EditSecondAxis(Action1);
        
        
        mouse.Move(NextXaxis,NextYaxis);
        ///****////
        countMoves++;
        ShowData();

        QValue = State1.Action[Action1];
        

        yield return new WaitForSeconds(Speed);

        //Starts the Sarsa Algorithm
        while (countEpisodes <= Episodes)
        {
            ShowData();

            print("Move From  " + Xaxis + " , " + Yaxis + " To " + NextXaxis + " , " + NextYaxis);
            Vector3 poss = mouse.GetPossision();
            print("Mouse Poss: " + poss);


            int pickAction = Random.Range(1,11);
            print("e-Greedy " +pickAction);
           

            if (pickAction <= eGreddy)
            {
                Action2 = DoRandomAction();
            }
            else
            {
                Action2 = GetMaxAction();
            }
            
            

            print("Choosen Action: " + Action2);
            State2 = state[NextXaxis, NextYaxis];
            qValue = State2.Action[Action1];

            //! SARSA !/
            QValue = QValue + Alpha * (State2.Reward + (Gamma * qValue) - QValue);
         

            
            state[Xaxis, Yaxis].Action[Action1] = QValue;

            
            //if this is Final Episode
            if (countEpisodes == Episodes)
            {
                ShowResult();
            }


            State1 = State2;
            Xaxis = NextXaxis;
            Yaxis = NextYaxis;
            Action1 = Action2;
            EditSecondAxis(Action1);
            QValue = qValue;

            //Move Mouse
            mouse.Move(NextXaxis, NextYaxis);

            //Check if mouse got to the goal
            if ((Xaxis == 3 & Yaxis == 2) || (Xaxis == 2 & Yaxis == 1) || (Xaxis == 1 & Yaxis == 3))
            {
                print("Goal Reached");
                countEpisodes++;
                
                RestartPoss();
            }

            if (countEpisodes == Episodes)
            {
                eGreddy = 0;
            }
            else if (countEpisodes > AfterEpisodes1)
            {
                eGreddy = AfterEGreddy1;
            }
            

            
            countMoves++;
            yield return new WaitForSeconds(Speed);
        }


        yield return null;
    }


    private string DoRandomAction()
    {
        //An tha kinithei sto aksona X i ston aksona Y
        // if XYmove = 0 tha kinithei sto akona X...diaforetika sto akona Y
        int XYmove = Random.Range(0, 2);

        int nextMove = Random.Range(1, 3);


        if (XYmove == 0)
        {

            if (nextMove == 1)
            {
                
                return "Left";
            }

            //else if (nextMove == 0)
            //{ this.gameObject.transform.position += Vector3.zero; }

            else if (nextMove == 2)
            {
                
                return "Right";
            }
        }

        else if (XYmove == 1)
        {
            if (nextMove == 1)
            {
                
                return "Down";
            }

            //else if (nextMove == 0)
            //{ this.gameObject.transform.position += Vector3.zero; }

            else if (nextMove == 2)
            {
                
                return "Up";
            }

        }

        return "null";
    }

    private string GetMaxAction()
    {
        string action = "Up";
       



        if (state[NextXaxis, NextYaxis].Action[action] < state[NextXaxis, NextYaxis].Action["Left"])
        {
            action = "Left";
        }
        if (state[NextXaxis, NextYaxis].Action[action] < state[NextXaxis, NextYaxis].Action["Right"])
        {
            action = "Right";
        }
        if (state[NextXaxis, NextYaxis].Action[action] < state[NextXaxis,NextYaxis].Action["Down"])
        {
            action = "Down";
        }
       
        
        return action;
    }


    private string EditSecondAxis(string action)
    {
       
        
        if (action == "Left")
        {
            NextXaxis = Mathf.Clamp(NextXaxis - 1, minAxis, maxAxis);
        }
        else if (action == "Right")
        {
            NextXaxis = Mathf.Clamp(NextXaxis + 1, minAxis, maxAxis);
        }
        else if (action == "Down")
        {
            NextYaxis = Mathf.Clamp(NextYaxis - 1, minAxis, maxAxis);
        }
        else if (action == "Up")
        {
            NextYaxis = Mathf.Clamp(NextYaxis + 1, minAxis, maxAxis);
        }

        return "null";
    }

    private void RestartPoss()
    {
        Xaxis = 0;
        Yaxis = 0;
        NextXaxis = 0;
        NextYaxis = 0;


        int pickAction = Random.Range(0, 11);
        print("e-Greedy " + pickAction);
        if (pickAction < eGreddy + 1)
        {
            Action1 = DoRandomAction();
        }
        else
        {
            Action1 = GetMaxAction();
        }


        State1 = state[Xaxis, Yaxis];
        EditSecondAxis(Action1);

        
        QValue = State1.Action[Action1];
        mouse.GoToStart();
    }

    private void ShowResult()
    {

        string action = GetMaxAction();
        int Zrotation = 0;
        if (action == "Left")
        {
            Zrotation = 180;
        }
        else if (action == "Right")
        {
            Zrotation = 0;
        }
        else if (action == "Down")
        {
            Zrotation = -90;
        }
        else if (action == "Up")
        {
            Zrotation = 90;
        }
        mouse.InstantiateFootprints(NextXaxis, NextYaxis, Zrotation);

    }

    public void ShowSpeed(float _speed)
    {
        Speed = _speed;
        _speed = float.Parse(_speed.ToString("n1"));
        SpeedText.text = _speed + "/sec for move";
    }

    public void ShowData()
    {
        EpisodesCarriedOut.text = countEpisodes.ToString();
        EpisodesLeft.text = (Episodes - countEpisodes).ToString();
        Moves.text = countMoves.ToString();
        eGreddyText.text = eGreddy.ToString();
    }

    
    public void WriteCSV()
    {
        string filePath = Application.dataPath + "/QTable.txt";

        if (!File.Exists(filePath))
        {
            using (StreamWriter sw = File.CreateText(filePath)) ;
        }

        StreamWriter writer = new StreamWriter(filePath);

        for (int y = 0; y < 4; y++)
        {
            for (int i = 0; i < 4; i++)
            {
                writer.WriteLine("Q" +state[i,y].Qnumber + ":  " + " UP " + state[i, y].Action["Up"] + " , " + " Down " + state[i, y].Action["Down"]
                + " , " + " Right " + state[i, y].Action["Right"] + " , " + " Left " + state[i, y].Action["Left"]);

            }
        }
        writer.WriteLine("Episodes" + countEpisodes);
        writer.Flush();
        writer.Close();
    }



}





    

[System.Serializable]
public class State
{
    
    public Dictionary<string, float> Action;
    public float Reward;
    public int Qnumber;

   public State()
    {
        Action = new Dictionary<string, float>();
        Action.Add("Up", 0);
        Action.Add("Down", 0);
        Action.Add("Left", 0);
        Action.Add("Right", 0);
    }

}