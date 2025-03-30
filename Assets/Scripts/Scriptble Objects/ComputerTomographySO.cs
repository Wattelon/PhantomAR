using UnityEngine;

[CreateAssetMenu(fileName = "ComputerTomographySO", menuName = "Scriptable Objects/ComputerTomography")]
public class ComputerTomographySO : TomographySO
{
    private protected override void Reset()
    {
        tomography = Tomography.Computer;
        base.Reset();
    }
}