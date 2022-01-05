using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Bishop : Chessman
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
        if (other.tag == "TargetEnemy")
        {
            anim = GetComponent<Animator>();
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

        //TopLeft
        i = CurrentX;
        j = CurrentZ;
        while (true)
        {
            i--;
            j++;
            if (i < 0 || j >= 8) break;
            c = BM.Instance.Chessmans[i, j];
            if (c == null)
                r[i, j] = true;

            else
            {
                if (c.isWhite != isWhite) r[i, j] = true;
                break;
            }

        }

        //TopRight
        i = CurrentX;
        j = CurrentZ;
        while (true)
        {
            i++;
            j++;
            if (i >= 8 || j >= 8) break;
            c = BM.Instance.Chessmans[i, j];
            if (c == null)
                r[i, j] = true;

            else
            {
                if (c.isWhite != isWhite) r[i, j] = true;
                break;
            }

        }

        //DownLeft
        i = CurrentX;
        j = CurrentZ;
        while (true)
        {
            i--;
            j--;
            if (i < 0 || j < 0) break;
            c = BM.Instance.Chessmans[i, j];
            if (c == null)
                r[i, j] = true;

            else
            {
                if (c.isWhite != isWhite) r[i, j] = true;
                break;
            }

        }

        //downRight

        i = CurrentX;
        j = CurrentZ;
        while (true)
        {
            i++;
            j--;
            if (j < 0 || i >= 8) break;
            c = BM.Instance.Chessmans[i, j];
            if (c == null)
                r[i, j] = true;

            else
            {
                if (c.isWhite != isWhite) r[i, j] = true;
                break;
            }

        }

        return r;
    }
}