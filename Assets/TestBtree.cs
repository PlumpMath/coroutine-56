using System;
using System.Collections.Generic;
using FluentBehaviourTree;
using UnityEngine;
using System.Threading;

public class TestBtree : MonoBehaviour
{

    string report = "";
    BehaviourTreeBuilder treeBuilder;
    IBehaviourTreeNode btree1;
    Dictionary<string, string> callData;
    private bool done = false;

    string seq1Action1 = "Sequence1Action1";
    string seq1Action2 = "Sequence1Action2";

    string sel1Condition = "Selector1Condition";
    string sel1Action1 = "Selector1Action1";
    string sel1Action2 = "Selector1Action2";
    string sel1Action3 = "Selector1Action3";

    string par1Action1 = "Paralle1Action1";
    string par1Action2 = "Paralle1Action2";
    string par1Action3 = "Paralle1Action3";

    string seq2Action1 = "Sequence2Action1";
    string seq2Action2 = "Sequence2Action2";
    string seq2Action3 = "Sequence2Action3";

    string FevalActionTrue = "evalActionTrue";
    string FevalActionFalse = "evalActionFalse";
    string FactionSuccess = "actionSuccess";
    string FactionFail = "actionFail";

    void Start()
    {
        Init();
        Log("Before StartCoroutine()");
        StartCoroutine(TickTree("Some Input Value", (myReturnValue, location) => {
            Log("Returned Value from " + location + " is: " + myReturnValue);
        }));
        Log("After StartCoroutine()");
    }

    IEnumerator<BehaviourTreeStatus> TickTree(string inputval, System.Action<BehaviourTreeStatus, string> callback)
    {
        Log("Beginning of TickTree()");
        for (var e = btree1.Tick(new TimeData(Time.deltaTime)); e.MoveNext();)
        {
            yield return e.Current;
            callback(e.Current, "Callback From TickTree()");
        }
       
        Log("End of TickTree() - Done=true");
        done = true;
        
        Log("--> Tree Hierarchy after:");
        Log(btree1.getTreeAsString(" --> "));
        Debug.Log(report);
    }

    void Update()
    {
        if (!done)
            Log("---> Called from Unity Update !! .. Delta Time is: "+Time.deltaTime);
    }

    void Log(string msg)
    {
        report += Time.frameCount + ": " + msg + "\n";
    }
    void Init()
    {
        treeBuilder = new BehaviourTreeBuilder();
        callData = new Dictionary<string, string>();
        initTree1();
    }

    void initTree1()
    {
       Log("Initializing Behavior Tree 1");

        btree1 = treeBuilder
                .Sequence("Sequence1")
                    .Do(seq1Action1, t =>
                    {
                        return actionSuccess(t, seq1Action1);
                    })
                    .Do(seq1Action2, t =>
                    {
                        return actionSuccess(t, seq1Action2);
                    })
                .End()
                .Selector("Selector-With-Condition")
                    .Condition(sel1Condition, t => { return evalActionTrue(t, sel1Condition); })
                        .Do(sel1Action1, t => { return actionFail(t, sel1Action1); })
                        .Do(sel1Action2, t => { return actionFail(t, sel1Action2); })
                        .Do(sel1Action3, t => { return actionFail(t, sel1Action3); })

                .Parallel("Parallel1", 1, 1).
                    Do(par1Action1, t =>
                    {
                        return actionFail(t, par1Action1);
                    })
                    .Do(par1Action2, t =>
                    {
                        return actionFail(t, par1Action2);
                    })
                    .Do(par1Action3, t =>
                    {
                        return actionSuccess(t, par1Action3);
                    })
                .End()
                .End()
                .Sequence("Sequence2")
                    .Do(seq2Action1, t =>
                    {
                        return actionSuccess(t, seq2Action1);
                    })
                    .Do(seq2Action2, t =>
                    {
                        return actionSuccess(t, seq2Action2);
                    })
                    .Do(seq2Action3, t =>
                    {
                        return actionSuccess(t, seq2Action3);
                    })
                .End()
            .Build();
       Log("Finished Buidling Behavior Tree 1 !");
    }

    public bool evalActionTrue(TimeData t, string aValue)
    {
        callData.Add(aValue + t.deltaTime, FevalActionTrue);
        return true;
    }
    public bool evalActionFalse(TimeData t, string aValue)
    {
        callData.Add(aValue + t.deltaTime, FevalActionFalse);
        return false;
    }
    public IEnumerator<BehaviourTreeStatus> actionSuccess(TimeData t, string aValue)
    {
       Log(aValue + " --> Action Successful ! at Delta time:" + t.deltaTime);
        callData.Add(aValue + t.deltaTime, FactionSuccess);
       Log("Return Status Running from actionSuccess ! - coroutine will return and then restart after this point.");
        yield return BehaviourTreeStatus.Running;
       // int milliseconds = 100;
       // Thread.Sleep(milliseconds);
        yield return BehaviourTreeStatus.Success;
    }
    public IEnumerator<BehaviourTreeStatus> actionFail(TimeData t, string aValue)
    {
       Log(aValue + " --> Action Failed ! at Delta time:" + t.deltaTime);
        //throw new ApplicationException("Node Failure to Execute !!");
        callData.Add(aValue + t.deltaTime, FactionFail);
       Log("Return Status Running from actionFail ! - coroutine will return and then restart after this point.");
        yield return BehaviourTreeStatus.Running;
       // int milliseconds = 100;
       // Thread.Sleep(milliseconds);
        yield return BehaviourTreeStatus.Failure;
    }

}
