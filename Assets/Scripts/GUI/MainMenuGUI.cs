using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GUI
{
    public class MainMenuGUI : MonoBehaviour
    {
        public GameObject canvas;
        public GameObject leaderboardPrefab;
        private GameObject _leaderboard;
        private bool _isLeaderboardOpen;
        public void OnPlayButton()
        {
            Debug.Log("OnPlayButton");
            SceneManager.LoadScene(1);
        }

        public void OnLeaderboardButton()
        {
            Debug.Log("OnLeaderboardButton");
            if (_isLeaderboardOpen)
            {
                OnLeaderboardBackButton();
                return;
            }
            _isLeaderboardOpen = true;
            _leaderboard = Instantiate(leaderboardPrefab, canvas.transform);
            GameObject.Find("Back_BTN").GetComponent<Button>().onClick.AddListener(OnLeaderboardBackButton); 
        }
        
        public void OnLeaderboardBackButton()
        {
            if (!_isLeaderboardOpen) return;
            _isLeaderboardOpen = false;
            Destroy(_leaderboard.gameObject);
            Debug.Log("OnLeaderboardBackButton");
        }

        public void OnExitButton()
        {
            Debug.Log("OnExitButton");
            Application.Quit(0);
        }
    }
}