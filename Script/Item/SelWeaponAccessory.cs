using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelWeaponAccessory : MonoBehaviour
{
    public Button[] Sel_Btn;

    public Image[] Sel_Img;
    public Image[] Sel_Frame;

    public GameObject LevelUpPanel;

    public Text[] ItemNameText;
    public Text[] ItemDescText;

    public GameObject[] NewItemText;

    private bool[] ThisItemType;

    // 선택된 인덱스를 추적하기 위한 리스트
    List<int> usedIndices = new();

    // Start is called before the first frame update
    void Start()
    {
        Sel_Btn[0].onClick.AddListener(() => SelWA(0));
        Sel_Btn[1].onClick.AddListener(() => SelWA(1));
        Sel_Btn[2].onClick.AddListener(() => SelWA(2));

    }

    private void OnEnable()
    {
        ThisItemType = new bool[3];

        ThisItemType[0] = false;
        ThisItemType[1] = false;
        ThisItemType[2] = false;

        ShowItem();

        if (LevelUpPanel.activeSelf)
        {
            Timer.Instance.PauseTimer(); // 레벨업 상태에서 타이머 멈춤
            Time.timeScale = 0; // 게임 정지
        }
    }

    private void OnDisable()
    {
        if (!LevelUpPanel.activeSelf)
        {
            Timer.Instance.ResumeTimer(); // 레벨업 상태 종료 시 타이머 재개
            Time.timeScale = 1; // 게임 재개
        }
    }

    // 선택된 옵션에 따라 무기 또는 악세서리를 처리하는 메서드
    private void SelWA(int option)
    {
        int CurrentItemNumber = usedIndices[option];
        string itemName;

        // 무기가 있을 때
        if (ThisItemType[option] == true)
        {
            itemName = ItemData.Instance.ItemDataName(CurrentItemNumber);
            if (GameManager.Instance.HavingSkill[CurrentItemNumber].IsOwned)
            {
                // 이미 존재하는 경우: 레벨을 1 증가 (최대 레벨 7 제한)
                var currentData = GameManager.Instance.HavingSkill[CurrentItemNumber];

                // 최대 레벨을 넘지 않도록 체크
                int newLevel = Mathf.Min(currentData.Item2 + 1, 7); // 최대 레벨 7
                GameManager.Instance.HavingSkill[CurrentItemNumber] = (true, newLevel);

                Debug.Log("무기가 있습니다.");

            }

            // 무기를 처음 뽑았을 때
            else
            {
                // 존재하지 않는 경우: 새로 추가하고 레벨을 1로 설정
                GameManager.Instance.HavingSkill[CurrentItemNumber] = (true, 1);
                GameManager.Instance.HavingSkill_Img[GameManager.Instance.HavingSkillNum].gameObject.SetActive(true);

                // N번 박스에는 Skeleton의 무기가 들어있습니다.
                if (!GameManager.Instance.WhereInSkill.ContainsKey(itemName))
                {
                    GameManager.Instance.WhereInSkill.Add(itemName, GameManager.Instance.HavingSkillNum);
                }
                GameManager.Instance.HavingSkillNum += 1;

                Debug.Log("무기가 없습니다.");
            }

            GameManager.Instance.HavingSkill_Img[GameManager.Instance.WhereInSkill[itemName]].sprite = GameManager.Instance.Sel_Sprite[CurrentItemNumber * 7 + GameManager.Instance.HavingSkill[CurrentItemNumber].Level - 1];
        }
        // 악세서리
        else
        {
            itemName = AccessoryData.Instance.AccessoryDataName(CurrentItemNumber);
            if (GameManager.Instance.HavingAccessory[CurrentItemNumber].IsOwned)
            {
                // 이미 존재하는 경우: 레벨을 1 증가 (최대 레벨 7 제한)
                var currentData = GameManager.Instance.HavingAccessory[CurrentItemNumber];

                // 최대 레벨을 넘지 않도록 체크
                int newLevel = Mathf.Min(currentData.Item2 + 1, 7); // 최대 레벨 7
                GameManager.Instance.HavingAccessory[CurrentItemNumber] = (true, newLevel);

                Debug.Log("무기가 있습니다.");

            }

            // 악세서리를 처음 뽑았을 때
            else
            {
                // 존재하지 않는 경우: 새로 추가하고 레벨을 1로 설정
                GameManager.Instance.HavingAccessory[CurrentItemNumber] = (true, 1);
                GameManager.Instance.HavingAccessory_Img[GameManager.Instance.HavingAccessoryNum].gameObject.SetActive(true);

                // N번 박스에는 Skeleton의 악세서리가 들어있습니다.
                if (!GameManager.Instance.WhereInAccessory.ContainsKey(itemName))
                {
                    GameManager.Instance.WhereInAccessory.Add(itemName, GameManager.Instance.HavingAccessoryNum);
                }
                GameManager.Instance.HavingAccessoryNum += 1;

                Debug.Log("악세서리가 없습니다.");
            }
            GameManager.Instance.HavingAccessory_Img[GameManager.Instance.WhereInAccessory[itemName]].sprite = GameManager.Instance.Sel_Accessory_Sprite[CurrentItemNumber * 7 + GameManager.Instance.HavingAccessory[CurrentItemNumber].Level - 1];
            Debug.Log("현재 usedIndices[option] : " + CurrentItemNumber);

        }
        Debug.Log("");
        Debug.Log("==============================================");
        Debug.Log("현재 뽑은 아이템 이름 : " + itemName);
        if (ThisItemType[option] == true) Debug.Log("현재 뽑은 스킬 레벨 : " + GameManager.Instance.HavingSkill[CurrentItemNumber].Level);
        if (ThisItemType[option] == false) Debug.Log("현재 뽑은 악세서리 레벨 : " + GameManager.Instance.HavingAccessory[CurrentItemNumber].Level);
        Debug.Log("==============================================");
        Debug.Log("");

        LevelUpPanel.SetActive(false);
    }


    // 아이템 보여주기 ( 3가지 )
    public void ShowItem()
    {
        usedIndices.Clear();
        int attempt = 0; // 시도 횟수 제한
        int maxAttempts = 100;

        for (int i = 0; i < Sel_Img.Length; i++)
        {
            int randomInt;
            int WhatRandomType;
            bool validItemFound = false; // 조건을 만족하는 아이템을 찾았는지 여부

            // 조건에 부합하는 아이템 찾기
            do
            {
                randomInt = Random.Range(0, 2);
                WhatRandomType = Random.Range(0, 2); // 0 = Skill,    1 = Accessory        
                attempt++;

                if (attempt > maxAttempts)
                {
                    Debug.LogWarning("조건을 만족하는 아이템이 부족합니다.");
                    break; // 더 이상 찾지 않고 루프 탈출
                }

                // 조건에 부합하는 경우
                if (!usedIndices.Contains(randomInt))
                {
                    if (WhatRandomType == 0)
                    {
                        if(GameManager.Instance.HavingSkill[randomInt].Level < 7)
                        {
                            validItemFound = true;
                            ThisItemType[i] = true;
                        }
                    }
                    else
                    {
                        if (GameManager.Instance.HavingAccessory[randomInt].Level < 7)
                        {
                            validItemFound = true;
                            ThisItemType[i] = false;
                        }
                    }
                }
            } while (!validItemFound);

            // 유효한 아이템을 찾지 못한 경우 루프 종료
            if (!validItemFound) break;
            usedIndices.Add(randomInt);

            Debug.Log("randomInt : " + randomInt);
            Debug.Log("WhatRandomType : " + WhatRandomType);

            // Skill
            if (ThisItemType[i] == true)
            {
                // 유효한 아이템을 추가
                Sel_Img[i].sprite = GameManager.Instance.Sel_Sprite[randomInt * 7 + GameManager.Instance.HavingSkill[randomInt].Level];
                ItemNameText[i].text = ItemData.Instance.ItemDataName(randomInt);
                ItemDescText[i].text = ItemData.Instance.ItemDataDesc(randomInt, GameManager.Instance.HavingSkill[randomInt].Level);

                Sel_Frame[i].color = ColorManager.ItemLevelColor[GameManager.Instance.HavingSkill[randomInt].Level];

                Outline outline = Sel_Frame[i].GetComponent<Outline>();
                outline.effectColor = ColorManager.ItemLevelColor[GameManager.Instance.HavingSkill[randomInt].Level];

                if (GameManager.Instance.HavingSkill[randomInt].Level == 0) NewItemText[i].SetActive(true);
                else NewItemText[i].SetActive(false);
            }

            // Accessory
            else
            {
                // 유효한 아이템을 추가
                Sel_Img[i].sprite = GameManager.Instance.Sel_Accessory_Sprite[randomInt * 7 + GameManager.Instance.HavingAccessory[randomInt].Level];
                ItemNameText[i].text = AccessoryData.Instance.AccessoryDataName(randomInt);
                ItemDescText[i].text = AccessoryData.Instance.AccessoryDataDesc(randomInt, GameManager.Instance.HavingAccessory[randomInt].Level);

                Sel_Frame[i].color = ColorManager.ItemLevelColor[GameManager.Instance.HavingAccessory[randomInt].Level];

                Outline outline = Sel_Frame[i].GetComponent<Outline>();
                outline.effectColor = ColorManager.ItemLevelColor[GameManager.Instance.HavingAccessory[randomInt].Level];

                if (GameManager.Instance.HavingAccessory[randomInt].Level == 0) NewItemText[i].SetActive(true);
                else NewItemText[i].SetActive(false);
            }
        }

        // 나머지 이미지 및 텍스트 비우기
        for (int i = usedIndices.Count; i < Sel_Img.Length; i++)
        {
            Sel_Btn[i].gameObject.SetActive(false);
        }
    }

}
