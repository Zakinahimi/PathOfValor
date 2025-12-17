using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour 
{
	#region Member Variables
	/// <summary>
	/// The player transform.
	/// </summary>
	private Transform playerTransform;

	/// <summary>
	/// The sprite renderer.
	/// </summary>
	private SpriteRenderer spriteRenderer;
	#endregion

	// Use this for initialization
	void Start () 
	{
		// obtain the local references
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player == null) player = GameObject.Find("Player");
		if (player == null) player = GameObject.Find("PlayerCharacter");
		if (player != null)
		{
			playerTransform = player.transform;
		}
		else
		{
			Debug.LogError("Tile could not find a player in the scene. Tag the player as 'Player' or name it 'Player'.");
		}

		spriteRenderer = this.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (playerTransform == null || spriteRenderer == null) return;

		// to ensure correct positioning of the environment around the player (3D Depth Effect)
		// we need to make the tiles below the player higher than the player in the render layering
		// and the ones above the player be lower than the player in the render layering
		if(this.tag == "LargeTile")
		{
			if((playerTransform.position.y - 1.28f) > this.transform.position.y)
			{
				// make all the tiles lower than the player higher than them on render layer 
				spriteRenderer.sortingLayerName = "TreeLayer";
			}
			else
			{
				// give this tile a normal tile set render order
				spriteRenderer.sortingLayerName = "Tileset";
			}
		}
	}
}
