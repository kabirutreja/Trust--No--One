using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;


public class GameManager : MonoBehaviour
{
    [Header("Trust Vignette")]
[SerializeField] private PostProcessVolume postProcessVolume;
[SerializeField] private float maxVignetteIntensity = 0.5f;

private Vignette vignette;
    
    [Header("Effects")]
[SerializeField] private ParticleSystem diamondParticlePrefab;
[SerializeField] private float diamondParticleLifetime = 1f;

[SerializeField] private GameObject bombExplosionPrefab; // has its own Animator with the explosion clip
[SerializeField] private Transform screenCenterPoint;     // empty GameObject placed at screen center
[SerializeField] private float bombExplosionLifetime = 2f; // should roughly match your explosion animation length
    [Header("Audio")]
    
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip cardFlipClip;
    [SerializeField] private AudioClip markPingClip;
    [SerializeField] private AudioClip swap;
    
    [SerializeField] private AudioClip WinClip;
    [SerializeField] private AudioClip LoseClip;


    [SerializeField] private Animator blueAnimator;   // the blue GameObject's Animator
[SerializeField] private Animator pinkAnimator; 






    public Shake camera;
    [SerializeField] private int minShuffleCount = 6; 
    [Header("Round Display")]
[SerializeField] private TextMeshProUGUI roundText;
    private bool challengeAvailable = false;
    [Header("Game Over / Win")]
[SerializeField] private GameObject gameOverUI;
[SerializeField] private GameObject gameWinUI;

private bool gameOver = false;

private GameObject aiSelectedCard;

    [Header("Challenge Button")]
[SerializeField] private GameObject challengeButton;
[SerializeField] private Animator challengeButtonAnimator;
 [Header("AI Marking")]
[SerializeField] private float startingMarkAccuracy = 100f;
[SerializeField] private float markAccuracyDecrease = 2f;
[SerializeField] private float markDisplayTime = 5f;

private float currentMarkAccuracy;

// This remembers the card even after its Mark object disappears
private GameObject markedCard;
    [Header("Cards")]
    [SerializeField] private GameObject[] cards = new GameObject[25];

    [Header("Initial Card Sequence")]
    [SerializeField] private float startDelay = 2f;
    [SerializeField] private float showTime = 3f;
    [SerializeField] private float hideDelay = 0.5f;

    [Header("Turn Settings")]
    [SerializeField] private float startingSafeProbability = 100f;
    [SerializeField] private float probabilityDecrease = 1f;

    [Header("Turn Delays")]
    [SerializeField] private float aiDelay = 2f;

    [Header("Shuffle Settings")]
    [SerializeField] private int shuffleCount = 12;
    [SerializeField] private float firstRoundShuffleTime = 0.6f;
    [SerializeField] private float shuffleSpeedIncrease = 0.05f;
    [SerializeField] private float delayBetweenSwaps = 0.05f;
    [Header("Turn Indicator")]
[SerializeField] private Sprite playerNormalSprite;
[SerializeField] private Sprite playerGlowSprite;

[SerializeField] private Sprite enemyNormalSprite;
[SerializeField] private Sprite enemyGlowSprite;

[SerializeField] private SpriteRenderer playerTurnRenderer;
[SerializeField] private SpriteRenderer enemyTurnRenderer;
private HashSet<GameObject> revealedCards = new HashSet<GameObject>();

    private float currentSafeProbability;

    private int turnNumber = 0;

    private bool playerTurn = false;
    private bool gameStarted = false;
    private bool cardBeingPicked = false;

    private void Start()
{
    UpdateRoundText();
    //HideChallengeButton();
    currentSafeProbability = startingSafeProbability;

    currentMarkAccuracy = startingMarkAccuracy;
    if (postProcessVolume != null && postProcessVolume.profile.TryGetSettings(out vignette))
{
    vignette.intensity.value = 0f;
}

    StartCoroutine(StartGameSequence());
}
private void UpdateTrustVignette()
{
    if (vignette == null) return;

    // currentMarkAccuracy: 100 = fully honest/trustworthy, 0 = fully untrustworthy
    float trustRatio = currentMarkAccuracy / 100f;          // 1 = full trust, 0 = no trust
    float distrustRatio = 1f - trustRatio;                   // 0 = full trust, 1 = no trust

    vignette.intensity.value = distrustRatio * maxVignetteIntensity;
}
private void PlaySFX(AudioClip clip)
{
    if (clip == null || sfxSource == null) return;
    sfxSource.pitch = Random.Range(0.92f, 1.08f);
    sfxSource.PlayOneShot(clip);
}
private void UpdateRoundText()
{
    if (roundText != null)
        roundText.text = "Round " + turnNumber;
}
private void PlayDiamondParticle(Transform cardTransform)
{
    if (diamondParticlePrefab == null) return;

    ParticleSystem effect = Instantiate(
        diamondParticlePrefab,
        cardTransform.position,
        Quaternion.identity
    );

    Destroy(effect.gameObject, diamondParticleLifetime);
}

private void PlayBombExplosion()
{
    if (bombExplosionPrefab == null) return;

    Vector3 spawnPosition = screenCenterPoint != null
        ? screenCenterPoint.position
        : Vector3.zero;

    GameObject explosion = Instantiate(
        bombExplosionPrefab,
        spawnPosition,
        Quaternion.identity
    );

    Destroy(explosion, bombExplosionLifetime);
}

    // ==================================================
    // INITIAL GAME SEQUENCE
    // ==================================================

    private IEnumerator StartGameSequence()
    {
    yield return new WaitForSeconds(startDelay);

    yield return StartCoroutine(ShowAllCardsRoutine()); // was: ShowAllCards();

    yield return new WaitForSeconds(showTime);

    yield return StartCoroutine(HideAllCardsRoutine()); // was: HideAllCards();

    yield return new WaitForSeconds(hideDelay);

    gameStarted = true;
    StartPlayerTurn();

    }
    private IEnumerator MarkAICard(GameObject aiSelectedCard)
    {
    float randomChance = Random.Range(0f, 100f);


    // Decide which card AI marks
    if (randomChance <= currentMarkAccuracy)
    {
        // AI marks the card it actually selected
        markedCard = aiSelectedCard;
    }
    else
    {
        // AI marks a different card
        markedCard = ChooseDifferentCard(aiSelectedCard);
    }
    PlaySFX(markPingClip);


    // Decrease accuracy for next round
    currentMarkAccuracy -= markAccuracyDecrease;

    currentMarkAccuracy = Mathf.Clamp(
        currentMarkAccuracy,
        0f,
        100f
    );
    UpdateTrustVignette();

    // Find the Mark child
    Transform mark = markedCard.transform.Find("Mark");

    if (mark == null)
    {
        Debug.LogWarning(
            "No child named 'Mark' found on " + markedCard.name
        );

        yield break;
    }

    // Get SpriteRenderer from Mark child
    SpriteRenderer markRenderer = mark.GetComponent<SpriteRenderer>();

    if (markRenderer == null)
    {
        Debug.LogWarning(
            "Mark child has no SpriteRenderer on " + markedCard.name
        );

        yield break;
    }

    // Enable mark
    markRenderer.enabled = true;

    // Play blink animation
    Animator animator = mark.GetComponent<Animator>();

    if (animator != null)
    {
        animator.SetTrigger("Blink");
    }

    // Keep mark visible for 5 seconds
    yield return new WaitForSeconds(markDisplayTime);

    // Disable mark
    markRenderer.enabled = false;
    animator.SetTrigger("NBlink");

    Debug.Log("Mark hidden, but remembered: " + markedCard.name);
}
private GameObject ChooseDifferentCard(GameObject originalCard)
{
    GameObject differentCard;

    do
    {
        int randomIndex = Random.Range(0, cards.Length);
        differentCard = cards[randomIndex];

    } while (differentCard == originalCard);

    return differentCard;
}

    // ==================================================
    // PLAYER TURN
    // ==================================================

    private void StartPlayerTurn()
    {
        if (!gameStarted)
            return;


        playerTurn = true;
        cardBeingPicked = false;
        SetPlayerTurnIndicator();

        Debug.Log("PLAYER TURN");
        Debug.Log("Safe Probability: " + currentSafeProbability + "%");
    }
private void ShowChallengeButton()
{
    //challengeButton.SetActive(true);

    challengeAvailable = true;

    challengeButtonAnimator.SetTrigger("Show");
}

private void HideChallengeButton()
{
    if (!challengeAvailable)
        return;
    challengeAvailable = false;

    challengeButtonAnimator.SetTrigger("Close");
}
public void Challenge()
{
 
    if (!playerTurn)
        return;

    if (gameOver)
        return;

    if (markedCard == null)
    {
        Debug.LogWarning("There is no marked card!");
        return;
    }

    if (aiSelectedCard == null)
    {
        Debug.LogWarning("AI has not selected a card!");
        return;
    }

    HideChallengeButton();

    Debug.Log("PLAYER CHALLENGED!");
    Debug.Log("AI selected: " + aiSelectedCard.name);
    Debug.Log("AI marked: " + markedCard.name);

    // Same card = AI was telling the truth
    if (markedCard == aiSelectedCard)
    {
        Debug.Log("CHALLENGE FAILED!");
        GameOver();
    }
    else
    {
        Debug.Log("CHALLENGE SUCCESSFUL!");
        GameWin();
    }
}
private void GameOver()
{
    if (gameOver)
        return;
        camera.Start = true;

    gameOver = true;
     PlayBombExplosion();

    playerTurn = false;
    cardBeingPicked = true;

    HideChallengeButton();

    Debug.Log("GAME OVER - AI WINS");

    if (gameOverUI != null)
    {
        gameOverUI.SetActive(true);
    }
    PlaySFX(LoseClip);
}
private void GameWin() 
{
    if (gameOver)
        return;

    gameOver = true;
    camera.Start = true;
     PlayBombExplosion();

    playerTurn = false;
    cardBeingPicked = true;

    HideChallengeButton();

    Debug.Log("GAME WON - PLAYER WINS");

    if (gameWinUI != null)
    {
        gameWinUI.SetActive(true);
    }
    PlaySFX(WinClip);
}


    public void PlayerPickedCard(GameObject selectedCard)
    {
        if (!playerTurn)
            return;

        if (cardBeingPicked)
            return;
            
            if (gameOver)
        return;
         if (revealedCards.Contains(selectedCard)) return;
                   if (!selectedCard.GetComponent<Card>().IsBomb)
    {
        

       
        
    
    
        PlayDiamondParticle(selectedCard.transform);
    }
    
        HideChallengeButton();

        cardBeingPicked = true;
        playerTurn = false;

        Debug.Log("Player selected: " + selectedCard.name);

        StartCoroutine(PlayerCardSequence(selectedCard));
    }

    private IEnumerator PlayerCardSequence(GameObject selectedCard)
    {
        // Show selected card
        ShowCard(selectedCard);
       // yield return new WaitForSeconds(showTime);

            Card card = selectedCard.GetComponent<Card>();
               if (card != null && card.IsBomb)
    {
        Debug.Log("PLAYER HIT A BOMB!");

        GameOver();
        yield break;
    }
    revealedCards.Add(selectedCard);
        

       // yield return new WaitForSeconds(showTime);

          
    
    
        // Hide selected card
        //HideCard(selectedCard);

        yield return new WaitForSeconds(hideDelay);

        // Decrease probability
        //DecreaseProbability();

        // Shuffle
        yield return StartCoroutine(ShuffleCards());

        // AI turn
        yield return StartCoroutine(StartAITurn());
    }

    // ==================================================
    // AI TURN
    // ==================================================

    private IEnumerator StartAITurn()
{
    yield return new WaitForSeconds(aiDelay);

    SetAITurnIndicator();

    Debug.Log("AI TURN");

    aiSelectedCard = ChooseAICard();

    // AI marks a card
     StartCoroutine(MarkAICard(aiSelectedCard));
    

    // Reveal the card AI actually picked
    //ShowCard(aiCard);

    yield return new WaitForSeconds(showTime);
    Card card = aiSelectedCard.GetComponent<Card>();

    if (card != null && card.IsBomb)
    {
        Debug.Log("AI HIT A BOMB!");

        GameWin();
        yield break;
    }

   // HideCard(aiCard);

    yield return new WaitForSeconds(hideDelay);

    DecreaseProbability();

    // Shuffle
    //yield return StartCoroutine(ShuffleCards());

    turnNumber++;
    UpdateRoundText();
    ShowChallengeButton();

    // Player turn
    StartPlayerTurn();
}

    // ==================================================
    // AI CARD SELECTION
    // ==================================================

   private GameObject ChooseAICard()
{
    float randomChance = Random.Range(0f, 100f);

    // AI chooses a DIAMOND
    if (randomChance < currentSafeProbability)
    {
        GameObject[] diamondCards = System.Array.FindAll(
            cards,
            card =>
            {
                Card cardScript = card.GetComponent<Card>();
                return cardScript != null && !cardScript.IsBomb;
            }
        );

        if (diamondCards.Length > 0)
        {
            return diamondCards[
                Random.Range(0, diamondCards.Length)
            ];
        }
    }

    // AI chooses a BOMB
    GameObject[] bombCards = System.Array.FindAll(
        cards,
        card =>
        {
            Card cardScript = card.GetComponent<Card>();
            return cardScript != null && cardScript.IsBomb;
        }
    );

    if (bombCards.Length > 0)
    {
        return bombCards[
            Random.Range(0, bombCards.Length)
        ];
    }

    return null;
}

    // ==================================================
    // SHUFFLE SYSTEM
    // ==================================================

    private IEnumerator ShuffleCards()
    {
        Debug.Log("SHUFFLING...");
        List<GameObject> shufflePool = new List<GameObject>();
        foreach (GameObject c in cards)
    {
        if (!revealedCards.Contains(c))
            shufflePool.Add(c);
    }

    if (shufflePool.Count < 2)
    {
        Debug.Log("Not enough cards left to shuffle.");
        yield break;
    }
 
        float shuffleTime = firstRoundShuffleTime -
                             (turnNumber * shuffleSpeedIncrease);
 
        shuffleTime = Mathf.Max(shuffleTime, 0.075f);
 
        // FIX 3: shuffle count itself now shrinks slightly as rounds go up,
        // so later rounds feel snappier and more intense instead of "same length, just faster."
        int currentShuffleCount = Mathf.Max(minShuffleCount, shuffleCount - turnNumber);
 
        for (int i = 0; i < currentShuffleCount; i++)
        {
            int firstIndex = Random.Range(0, shufflePool.Count);
        int secondIndex = Random.Range(0, shufflePool.Count);
 
            while (secondIndex == firstIndex)
            {
                secondIndex = Random.Range(0, shufflePool.Count);
            }
 
             GameObject firstCard = shufflePool[firstIndex];
        GameObject secondCard = shufflePool[secondIndex];
 
            PlaySFX(swap);
 
            yield return StartCoroutine(
                SwapCards(firstCard, secondCard, shuffleTime)
            );
 
            yield return new WaitForSeconds(delayBetweenSwaps);
        }
 
        Debug.Log("SHUFFLE COMPLETE");
    }

    // ==================================================
    // SMOOTH CARD SWAP
    // ==================================================

    private IEnumerator SwapCards(
        GameObject firstCard,
        GameObject secondCard,
        float duration)
    {
        Vector3 firstPosition = firstCard.transform.position;
        Vector3 secondPosition = secondCard.transform.position;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / duration;

            // Smooth movement
            t = Mathf.SmoothStep(0f, 1f, t);

            firstCard.transform.position =
                Vector3.Lerp(firstPosition, secondPosition, t);

            secondCard.transform.position =
                Vector3.Lerp(secondPosition, firstPosition, t);

            yield return null;
        }

        // Make absolutely sure they end at the correct positions
        firstCard.transform.position = secondPosition;
        secondCard.transform.position = firstPosition;
    }

    // ==================================================
    // CARD ANIMATIONS
    // ==================================================

   private IEnumerator ShowAllCardsRoutine()
{
    foreach (GameObject card in cards)
    {
        ShowCard(card);
        yield return new WaitForSeconds(0.05f); // small stagger, tune to taste
    }
}

private IEnumerator HideAllCardsRoutine()
{
    foreach (GameObject card in cards)
    {
        HideCard(card);
        yield return new WaitForSeconds(0.05f);
    }
}
 private void SetPlayerTurnIndicator()
{
    playerTurnRenderer.sprite = playerGlowSprite;
    enemyTurnRenderer.sprite = enemyNormalSprite;

    blueAnimator.SetBool("blue", true);
    pinkAnimator.SetBool("pink", false);
}

private void SetAITurnIndicator()
{
    playerTurnRenderer.sprite = playerNormalSprite;
    enemyTurnRenderer.sprite = enemyGlowSprite;

    blueAnimator.SetBool("blue", false);
    pinkAnimator.SetBool("pink", true);
}

    private void ShowCard(GameObject card)
    {
        Animator animator = card.GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetTrigger("Show");
        }
        PlaySFX(cardFlipClip);
    }

    private void HideCard(GameObject card)
    {
        Animator animator = card.GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetTrigger("Hide");
        }
        PlaySFX(cardFlipClip);
    }

    // ==================================================
    // PROBABILITY
    // ==================================================

    private void DecreaseProbability()
    {
        currentSafeProbability -= probabilityDecrease;

        currentSafeProbability =
            Mathf.Clamp(currentSafeProbability, 0f, 100f);

        Debug.Log(
            "Safe Probability: " +
            currentSafeProbability +
            "%"
        );
    }
}