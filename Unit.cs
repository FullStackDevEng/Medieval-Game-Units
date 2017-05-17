namespace MGG
{
    class Unit
    {
        #region To add/edit Units add here

        public const int WORKER = 0;
        public const int WARRIOR = 1;
        public const int ARCHER = 2;
        public const int MAGE = 3;
        public const int CATAPULT = 4;

        private static string[] TYPES = { "worker", "warrior", "archer", "mage", "catapult" };
        private static double[] SPEEDS = { 10, 30, 15, 10, 5 };
        private static int[] HPS = { 50, 200, 100, 50, 300 };
        private static int[] DAMAGE_STRENGTHS = { 5, 25, 10, 50, 100 };
        private static int[] ATTACKING_FREQUENCIES = { 2, 2, 1, 3, 7 };// seconds to wait before attacking each time
        private static int[] ATTACKING_RANGE = { 5, 5, 100, 120, 300 };// range to attack (in pixels)

        #endregion To add/edit Units add here

        #region constants/static variables
        private const int ATTACK_DIRECTION = 1;
        private const int ID_LOWER_RANGE = 1000;
        private const int ID_HIGHER_RANGE = 10000;

        private const int X_POSITION_LOWER_RANGE = 10;
        private const int X_POSITION_HIGHER_RANGE = 100;

        private const int Y_POSITION_LOWER_RANGE = 10;
        private const int Y_POSITION_HIGHER_RANGE = 100;

        public static System.Collections.Generic.List<Unit> UnitList = new System.Collections.Generic.List<Unit>();
        private static System.Random RandomInt { get; set; } = new System.Random();

        #endregion constants/static variables

        #region Constructor
        public Unit(int type)
        {
            this._id = RandomInt.Next(ID_LOWER_RANGE, ID_HIGHER_RANGE);
            this._xposition = RandomInt.Next(X_POSITION_LOWER_RANGE, X_POSITION_HIGHER_RANGE);
            this._yposition = RandomInt.Next(Y_POSITION_LOWER_RANGE, Y_POSITION_HIGHER_RANGE);
            this._type = TYPES[type];
            this._hp = HPS[type];
            this._speed = SPEEDS[type];
            this._damageStrength = DAMAGE_STRENGTHS[type];
            this._attackFrequency = ATTACKING_FREQUENCIES[type];
            this._attackRange = ATTACKING_RANGE[type];
            UpdateNearestEnemy();

            UnitList.Add(this);
        }
        #endregion constructor

        #region unit parameters
        public int _id { get; }
        public string _type { get; }
        public int _xposition { get; set; }
        public int _yposition { get; set; }
        public int _hp { get; set; }
        public double _speed { get; set; }
        public int _damageStrength { get; set; }
        public int _attackFrequency { get; set; }
        public int _attackRange { get; set; }
        public CPU_Unit _nearestEnemy { get; set; }
        #endregion unit parameters 

        #region public/protected methods
        public void ChangeSpeed(int percentageFactor)
        {
            double decimalFactor = percentageFactor / 100.0;           
            this._speed *= decimalFactor;
        }
        public void Move(int x, int y, bool retreat = false)// upgrade
        {
            int direction = ATTACK_DIRECTION;
            if (retreat) { direction *= -1; }
            this._xposition += x * direction;
            this._yposition += y * direction;
            UpdateNearestEnemy();
        }
        public void Attack(CPU_Unit victim = null)
        {
            if (victim == null) { UpdateNearestEnemy(); victim = this._nearestEnemy; }
            victim.TakeDamage(this._damageStrength);
        }
        public void TakeDamage(int inflictedDamage)
        {
            _hp = _hp - inflictedDamage;
            if (_hp <= 0) { dead(); }
            else
            {
                updateNearestEnemyForAllUnits();             
            }
        }

        #endregion public/protected methods

        #region private methods  
        private void dead()
        {
            UnitList.Remove(this);
        }
        private void updateNearestEnemyForAllUnits()
        {
            new System.Threading.Thread(() =>
            {
                foreach (Unit unit in UnitList)
                {
                    unit.UpdateNearestEnemy();
                }
            }).Start();
        }
        private void UpdateNearestEnemy(Unit refUnit = null)
        {
            new System.Threading.Thread(() =>
            {
                System.Threading.Thread.CurrentThread.IsBackground = true;
                if (refUnit == null) { refUnit = this; }
                double min = 10000;
                CPU_Unit closest = null;
                foreach (CPU_Unit enemyUnit in CPU_Unit.UnitList)
                {
                    double distance = getPythagoreanDitance(refUnit._xposition, refUnit._yposition, enemyUnit._xposition, enemyUnit._yposition);
                    if (distance < min) { min = distance; closest = enemyUnit; }
                }
                this._nearestEnemy = closest;
            }).Start();           
        }
        private static double getPythagoreanDitance(int x1, int y1, int x2, int y2)
        {
            return System.Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
        }
        #endregion private methods
    }
}
