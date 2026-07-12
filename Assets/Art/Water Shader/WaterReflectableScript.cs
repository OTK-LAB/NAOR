// 2016 - Damien Mayance (@Valryon)
// Source: https://github.com/valryon/water2d-unity/
using UnityEngine;

/// <summary>
/// Automagically create a water reflect for a sprite.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class WaterReflectableScript : MonoBehaviour
{
  #region Members

  [Header("Reflect properties")]
  public Vector3 localPosition = new Vector3(0, -0.25f, 0);
  public Vector3 localRotation = new Vector3(0, 0, -180);
  public Vector3 localScale = new Vector3(-1, 1, 1);
  [Tooltip("Optional: force the reflected sprite. If null it will be a copy of the source.")]
  public Sprite sprite;
  public string spriteLayer = "Default";
  public int spriteLayerOrder = -1;

  private SpriteRenderer spriteSource;
  private SpriteRenderer spriteRenderer;
  private Rigidbody2D rb;
  private bool playersReflection;
  private GameObject playerReflection;
  private bool reflectionPosChanged;
  #endregion

  #region Timeline

  public void Awake()
  {
    GameObject reflectGo = new GameObject("Water Reflect");
    reflectGo.transform.parent = this.transform;
    reflectGo.transform.localPosition = localPosition;
    reflectGo.transform.localRotation = Quaternion.Euler(localRotation);
    reflectGo.transform.localScale = localScale;
    //reflectGo.transform.localScale = new Vector3(-reflectGo.transform.localScale.x, reflectGo.transform.localScale.y, reflectGo.transform.localScale.z);

    spriteRenderer = reflectGo.AddComponent<SpriteRenderer>();
    spriteRenderer.sortingLayerName = spriteLayer;
    spriteRenderer.sortingOrder = spriteLayerOrder;
    
    spriteSource = GetComponent<SpriteRenderer>();
    if(this.gameObject.CompareTag("Player"))
    {
      playersReflection = true;
      reflectGo.transform.parent = null;
      rb = reflectGo.AddComponent<Rigidbody2D>();
      rb.gravityScale = 0;
      playerReflection = reflectGo;
    }
    else
      playersReflection = false;
  }
  private void Update() 
  {
    if(playersReflection)
    {
      Rigidbody2D playerRB = GetComponent<Rigidbody2D>();
      if(GetComponent<PlayerController>().isGrounded)
      {
        playerReflection.transform.position = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y - 4.3f);
      }
      if(!GameObject.Find("Boat").GetComponent<BoatScript>().inBoat)
      {
        rb.velocity = new Vector2(0, -playerRB.velocity.y);
      }
      else
      {
        rb.velocity = new Vector2(0,0);
        if(!reflectionPosChanged)
        {
          playerReflection.transform.position = new Vector2(playerReflection.transform.position.x, playerReflection.transform.position.y + 0.4f);
          reflectionPosChanged = true;
        }
        playerReflection.transform.position = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y - 3.25f);
        playerReflection.transform.position =  new Vector2(playerReflection.transform.position.x, playerReflection.transform.position.y + Mathf.Lerp(-0.5f, 0.5f, Mathf.PingPong(Time.time, 1)) * Time.deltaTime);
      }
        playerReflection.transform.position = new Vector2(this.gameObject.transform.position.x, playerReflection.transform.position.y);
        playerReflection.transform.localScale = new Vector2(-this.gameObject.transform.localScale.x, this.gameObject.transform.localScale.y);
    }
 
  }
  void OnDestroy()
  {
    if (spriteRenderer != null)
    {
      Destroy(spriteRenderer.gameObject);
    }
  }

  void LateUpdate()
  {
    if (spriteSource != null)
    {
      if (sprite == null)
      {
        spriteRenderer.sprite = spriteSource.sprite;
      }
      else
      {
        spriteRenderer.sprite = sprite;
      }
      spriteRenderer.flipX = spriteSource.flipX;
      spriteRenderer.flipY = spriteSource.flipY;
      spriteRenderer.color = spriteSource.color;
    }
  }

  #endregion
}