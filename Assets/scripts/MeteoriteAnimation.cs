using System.Collections;
using DG.Tweening;
using TheBlackCat.TrailEffect2D;
using UnityEngine;

public class MeteoriteAnimation : MonoBehaviour
{
    [SerializeField] private GameObject meteoriteSprite;
    [SerializeField] private Animator animator;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private GameObject trail;
    private float duration = 1f;
    private void Start()
    {
        meteoriteSprite.transform.position = meteoriteSprite.transform.position = transform.position + new Vector3(
                    Random.Range(-40f, 40f),
                    Random.Range(30f, 60f), 
                    0f
                );
        trailRenderer.Clear();
        TrailManager.Instance.StartTrail(trail);
        meteoriteSprite.transform.DOMove( transform.position, duration).SetEase(Ease.InQuad).OnComplete(() =>
        {
            meteoriteSprite.SetActive(false);
            animator.Play("Explosion");
            StartCoroutine(DestroyAfterAnimation());
        });
    }
    
    public void SetDuration(float duration)
    {
        this.duration = duration;
    }
    
    private IEnumerator DestroyAfterAnimation()
    {
        yield return null;

        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);

        TrailManager.Instance.StopTrail(trail);
        Destroy(gameObject);
    }
}