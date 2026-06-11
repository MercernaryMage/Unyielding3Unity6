using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelConfiguration
{
	public List<PositionConfiguration> players = new List<PositionConfiguration>();
	public List<EnemyConfiguration> enemies = new List<EnemyConfiguration>();
}

public class EnemyConfiguration
{
	public EnemyConfiguration(string enemyName, Direction facing, int x, int y)
	{
		this.enemyName = enemyName;
		positionConfiguration = new PositionConfiguration(facing, x, y);
	}
	public string enemyName;
	public PositionConfiguration positionConfiguration;
}

public class PositionConfiguration
{
	public PositionConfiguration(Direction facing, int x, int y)
	{
		this.facing = facing;
		this.x = x;
		this.y = y;
	}
	public Direction facing;
	public int x;
	public int y;
}
