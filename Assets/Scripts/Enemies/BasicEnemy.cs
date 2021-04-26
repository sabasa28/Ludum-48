public class BasicEnemy : Enemy
{
    void Update()
    {
        MoveTowardsPlayer();
    }

    protected override void Attack() {}
}
