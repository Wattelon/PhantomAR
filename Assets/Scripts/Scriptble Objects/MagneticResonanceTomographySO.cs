using UnityEngine;

[CreateAssetMenu(fileName = "MagneticResonanceTomographySO", menuName = "Scriptable Objects/MagneticResonanceTomography")]
public class MagneticResonanceTomographySO : TomographySO
{
    private protected override void Reset()
    {
        tomography = Tomography.MagneticResonance;
        base.Reset();
    }
}
