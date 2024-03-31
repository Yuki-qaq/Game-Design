

public class Target : Hittable
{
    public int pointsPerHit = 100;

    public bool useScriptablePoints = true;

    public IntVariableScriptable pointsVariable;

    public int points;

    protected override void Init()
    {
        base.Init();

        if (useScriptablePoints)
        {
            points = pointsVariable.Value;
        }
        else
        {
            points = 0;
        }
    }

    public override void Hit()
    {
        points += pointsPerHit;

        if (useScriptablePoints)
        {
            pointsVariable.Value = points;
        }

        base.Hit();
    }
}
