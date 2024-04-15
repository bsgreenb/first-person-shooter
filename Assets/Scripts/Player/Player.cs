using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int HP = 100;
    public GameObject bloodyScreen;

    public TextMeshProUGUI playerHealthUI;
    public GameObject gameOverUI;

    public bool isDead;

    private void Start()
    {
        playerHealthUI.text = $"Health: {HP}";
    }

    public void TakeDamage(int damageAmount)
   {
        HP -= damageAmount;

        if (HP <= 0) {
            print("Player Dead");
            PlayerDead();
            isDead = true;
            // Game over
            // ReSpawn Player
            // Dying Animation

        } else {
            print("Player Hit");
            StartCoroutine(BloodyScreenEffect());
            playerHealthUI.text = $"Health: {HP}";
            SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerHurt);
        }
   }

    private void PlayerDead()
    {
        SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerDie);
        GetComponent<InputManager>().DisablePlayerInput();
        // Dying animation
        GetComponentInChildren<Animator>().enabled =true;
        playerHealthUI.gameObject.SetActive(false);

        GetComponent<ScreenBlackout>().StartFade();
        StartCoroutine(ShowGameOverUI());

    }

    private IEnumerator ShowGameOverUI()
    {
        yield return new WaitForSeconds(1f);
        gameOverUI.gameObject.SetActive(true);

        int waveSurvived = GlobalReferences.Instance.waveNumber;
        if ((waveSurvived - 1) > SaveLoadManager.Instance.LoadHighScore()) {
            SaveLoadManager.Instance.SaveHighScore(waveSurvived - 1);
        }

        StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(4f);

        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator BloodyScreenEffect()
   {
        if (!bloodyScreen.activeInHierarchy) {
            bloodyScreen.SetActive(true);
        }

        var image = bloodyScreen.GetComponentInChildren<Image>();
 
        // Set the initial alpha value to 1 (fully visible).
        Color startColor = image.color;
        startColor.a = 1f;
        image.color = startColor;
 
        float duration = 4f;
        float elapsedTime = 0f;
 
        while (elapsedTime < duration)
        {
            // Calculate the new alpha value using Lerp.
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
 
            // Update the color with the new alpha value.
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;
 
            // Increment the elapsed time.
            elapsedTime += Time.deltaTime;
 
            yield return null; ; // Wait for the next frame.
        }



        if (bloodyScreen.activeInHierarchy) {
            bloodyScreen.SetActive(false);
        }
   }

   private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("ZombieHand")) {
            if (!isDead) {
                TakeDamage(other.gameObject.GetComponent<ZombieHand>().damage);
            }
        }
   }
}