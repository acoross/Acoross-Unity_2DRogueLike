using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour 
{
	public float levelStartDelay = 2f; 
	public float turnDelay = 0.1f;
	public static GameManagerScript instance = null;
	public BoardManager boardScript;
	public int playerFoodPoints = 100;
	[HideInInspector] public bool playersTurn = true;

	private Text levelText;
	private GameObject levelImage;
	private bool doingSetup;

	private int level = 1;
	private List<EnemyScript> enemies;
	private bool enemiesMoving;

	// Use this for initialization
	void Awake () 
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad (gameObject);
		enemies = new List<EnemyScript>();
		boardScript = gameObject.GetComponent<BoardManager>();
		InitGame();
	}

	private void OnLevelWasLoaded (int index)
	{
		level++;

		InitGame ();
		enemiesMoving = false;
	}

	void InitGame()
	{
		doingSetup = true;

		levelImage = GameObject.Find ("LevelImage");
		levelText = GameObject.Find ("LevelText").GetComponent<Text>();
		levelText.text = "Day " + level;
		levelImage.SetActive (true);

		print ("before invoke hidelevelimage " + Time.time);

		Invoke ("HideLevelImage", levelStartDelay);

		print ("after invoke hidelevelimage " + Time.time);

		boardScript.SetupScene(level);
		enemies.Clear ();
	}

	private void HideLevelImage()
	{
		levelImage.SetActive (false);
		doingSetup = false;
	}

	public void GameOver()
	{
		levelText.text = "After " + level + " days, you starved.";
		levelImage.SetActive(true);
		enabled = false;
	}

	void Update () 
	{
		if (playersTurn || enemiesMoving || doingSetup)
		{
			return;
		}

		StartCoroutine(MoveEnemies ());

	}

	public void AddEnemyToList(EnemyScript script)
	{
		enemies.Add (script);
	}

	IEnumerator MoveEnemies()
	{
		enemiesMoving = true;
		yield return new WaitForSeconds(turnDelay);

		if (enemies.Count == 0)
		{
			yield return new WaitForSeconds(turnDelay);
		}

		for (int i = 0; i < enemies.Count; i++)
		{
			enemies[i].MoveEnemy();
			yield return new WaitForSeconds(enemies[i].moveTime);
		}

		playersTurn = true;
		enemiesMoving = false;
	}
}