using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class King : Chessman
{

    public AudioSource audioSource;
    private Animator anim;
    public AudioSource dying;
    NavMeshAgent agent;
    void Start()
    {
        score = 900;
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
        Chessman c;
        int i, j;
        //TopSide
        i = CurrentX - 1;
        j = CurrentZ + 1;
        if (CurrentZ != 7)

        {

            for (int k = 0; k < 3; k++)
            {
                if (i >= 0 && i < 8)
                {
                    c = BM.Instance.Chessmans[i, j];
                    if (c == null) r[i, j] = true;
                    else if (isWhite != c.isWhite) r[i, j] = true;
                }
                i++;

            }
        }

        //Downside
        i = CurrentX - 1;
        j = CurrentZ - 1;
        if (CurrentZ != 0)

        {

            for (int k = 0; k < 3; k++)
            {
                if (i >= 0 && i < 8)
                {
                    c = BM.Instance.Chessmans[i, j];
                    if (c == null) r[i, j] = true;
                    else if (isWhite != c.isWhite) r[i, j] = true;
                }
                i++;

            }

        }

        ///middle left
        if (CurrentX != 0)
        {
            c = BM.Instance.Chessmans[CurrentX - 1, CurrentZ];
            if (c == null) r[CurrentX - 1, CurrentZ] = true;
            else if (isWhite != c.isWhite) r[CurrentX - 1, CurrentZ] = true;
        }
        ///middle right
        if (CurrentX != 7)
        {
            c = BM.Instance.Chessmans[CurrentX + 1, CurrentZ];
            if (c == null) r[CurrentX + 1, CurrentZ] = true;
            else if (isWhite != c.isWhite) r[CurrentX + 1, CurrentZ] = true;
        }
        return r;
    }
}