using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Horse : Chessman
{

    public AudioSource audioSource;
    private Animator anim;
    public AudioSource dying;
    NavMeshAgent agent;
    void Start()
    {
        score = 30;
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }

    public void dyingSound()
    {
        dying.Play();
    }
    public override void move(Vector3 pos)
    {
        agent.destination = pos;
    }
    private void OnTriggerEnter(Collider other)
    {
        anim = GetComponent<Animator>();

        if (other.tag == "TargetEnemy")
        {

            anim.SetTrigger("attack");
            audioSource.Play();
        }
        anim = GetComponent<Animator>();

        if (GetComponent<Animator>().tag == "TargetEnemy")
        {
            anim.SetTrigger("dying");
        }
    }
    public override bool[,] possibleMove()
    {


        bool[,] r = new bool[8, 8];

        // Upleft
        knightMove(CurrentX - 1, CurrentZ + 2, ref r);
        // UpRight
        knightMove(CurrentX + 1, CurrentZ + 2, ref r);
        // Rightup
        knightMove(CurrentX + 2, CurrentZ + 1, ref r);
        // RightDown
        knightMove(CurrentX + 2, CurrentZ - 1, ref r);

        // Downleft
        knightMove(CurrentX - 1, CurrentZ - 2, ref r);
        // DownRight
        knightMove(CurrentX + 1, CurrentZ - 2, ref r);
        // lefttup
        knightMove(CurrentX - 2, CurrentZ + 1, ref r);
        // lefttDown
        knightMove(CurrentX - 2, CurrentZ - 1, ref r);
        return r;
    }
    public void knightMove(int x, int y, ref bool[,] r)
    {
        Chessman c;
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            c = BM.Instance.Chessmans[x, y];
            if (c == null)
                r[x, y] = true;
            else if (isWhite != c.isWhite)
                r[x, y] = true;
        }
    }
}
