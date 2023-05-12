using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dashParticleManager : MonoBehaviour
{
    [SerializeField] ParticleSystem frontDash;
    [SerializeField] ParticleSystem leftDash;
    [SerializeField] ParticleSystem rightDash;
    [SerializeField] ParticleSystem backDash;

    [SerializeField] Transform frontParticlePos;
    [SerializeField] Transform leftParticlePos;
    [SerializeField] Transform rightParticlePos;
    [SerializeField] Transform backParticlePos;

    float x, y;
    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
    }

    public void manageDash()
    {
        if (y >= 0 && x == 0)
        {
            ParticleSystem particle = Instantiate(frontDash, frontParticlePos);
        }
        else if (y < 0)
        {
            ParticleSystem particle = Instantiate(backDash, backParticlePos);
        }
        else if (y == 0 && x < 0)
        {
            ParticleSystem particle = Instantiate(leftDash, leftParticlePos);
        }
        else if (y == 0 && x > 0)
        {
            ParticleSystem particle = Instantiate(rightDash, rightParticlePos);
        }
    }
}
