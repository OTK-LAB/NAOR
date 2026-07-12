using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateManager : MonoBehaviour
{
    public HealthGateListSO healthGateListSo;
    [SerializeField]private PlayerManager _playerManager;
    [SerializeField]private PlayerController _playerController;
    private float percentage;
    private void OnEnable()
    {
        
        Actions.OnHealthChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        Actions.OnHealthChanged -= OnHealthChanged;
    }

    void Start()
    {
        //ResetLists();
        ResetGems();
        ResetBuffs();
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        OnHealthChanged();
    }


    private void OnHealthChanged()
    {
        percentage = _playerManager.CurrentHealth*100 / _playerManager.MaxHealth;
           if (percentage>= healthGateListSo.HealthGateList[3].percentage)
                {
                    ResetBuffs();
                    foreach (var gems in  healthGateListSo.HealthGateList[3].activeGems)
                    {
                        if (gems != null)
                        {
                            GiveBuff(gems,gems.gemtype); 
                        }
                        
                    }
                    
                }
                else if (percentage>= healthGateListSo.HealthGateList[2].percentage)
                {
                    ResetBuffs();
                    foreach (var gems in  healthGateListSo.HealthGateList[2].activeGems)
                    {
                        if (gems != null)
                        {
                            GiveBuff(gems,gems.gemtype); 
                        }
                    }
                }
                else if (percentage>= healthGateListSo.HealthGateList[1].percentage)
                {
                    ResetBuffs();
                    foreach (var gems in  healthGateListSo.HealthGateList[1].activeGems)
                    {
                        if (gems != null)
                        {
                            GiveBuff(gems,gems.gemtype); 
                        }
                    }
                }
                else if (percentage>= healthGateListSo.HealthGateList[0].percentage)
                {
                    ResetBuffs();
                    foreach (var gems in  healthGateListSo.HealthGateList[0].activeGems)
                    {
                        if (gems != null)
                        {
                            GiveBuff(gems,gems.gemtype); 
                        }
                    }
                }
                
            
        
    }
    private void GiveBuff(GemSO gem ,GemSO.Gemtype gemtype)
    {
        switch (gemtype)
        {
            case GemSO.Gemtype.AttackBuff: //LEGEN
                Debug.Log("eski attack degeri " +_playerController.attackDamage);
                _playerController.attackDamage += _playerController.attackDamage * gem.buffRate / 100;
                Debug.Log(gem.buffRate + "Attack buff alindi");
                Debug.Log("yeni attack degeri " +_playerController.attackDamage);
                break;

            case GemSO.Gemtype.SpeedBuff: //LEGEN
                Debug.Log("eski runspeed degeri " +_playerController.runSpeed);
                Debug.Log("eski walkspeed degeri " +_playerController.walkSpeed);
                _playerController.runSpeed +=_playerController.runSpeed * gem.buffRate / 100;
                _playerController.walkSpeed +=_playerController.walkSpeed * gem.buffRate / 100;
                Debug.Log("TAHA BURADAYDI");
                
                Debug.Log(gem.buffRate + "speed buff alindi");
                Debug.Log("yeni runspeed degeri " +_playerController.runSpeed);
                Debug.Log("yeni walkspeed degeri " +_playerController.walkSpeed);
                break;

            case GemSO.Gemtype.DefenseBuff: //LEGEN
                _playerManager.defenceRate += gem.buffRate/100;
                break;

            case GemSO.Gemtype.Acrobatics: //LEGEN
                _playerController.rollStaminaCost -=  _playerController.rollStaminaCost * gem.buffRate / 100;
                _playerController.jumpForce += _playerController.jumpForce * gem.buffRate / 100;
                break;

            case GemSO.Gemtype.BloodThirsty: //LEGEN
                _playerController.lifeStealRate += gem.buffRate / 10;
                if(_playerController.isAttacking==true){
                    _playerManager.CurrentHealth += _playerController.lifeStealRate * _playerController.attackDamage;
                    }
                Debug.Log("%"+gem.buffRate + "Can calma alindi");
                Debug.Log(_playerController.lifeStealRate);
                break;

            case GemSO.Gemtype.PowerfulLaunch: //RARE
                Dagger.instance.DaggerDamageUp();
                break;

            case GemSO.Gemtype.Ghost: //RARE
                 Physics2D.IgnoreLayerCollision (7, 8, true);
                break;
            case GemSO.Gemtype.QuickHand: //RARE
                _playerController.cooldownTime = 6f;
                break;
            case GemSO.Gemtype.ShieldUp: // RARE
                _playerManager.shieldDefenceRate = gem.buffRate / 100;
                _playerController.shieldStamina = 0f;
                break;
            case GemSO.Gemtype.Vaulter: // RARE
                _playerController.rollSeconds += gem.buffRate / 100;
                break;
            case GemSO.Gemtype.Regen:// RARE
                _playerManager.regenHealth = gem.buffRate;
                _playerManager.isRegen = true;
                break;

        }
    }
    
    public void ResetGems()
    {
        foreach (var gems in healthGateListSo.HealthGateList[3].activeGems)
        {
            if (gems != null){
            gems.isActive = false;
            }


        }
        foreach (var gems in healthGateListSo.HealthGateList[2].activeGems)
        {

            if (gems != null){
            gems.isActive = false;
            }


        }
        foreach (var gems in healthGateListSo.HealthGateList[1].activeGems)
        {

           if (gems != null){
            gems.isActive = false;
            }


        }
        foreach (var gems in healthGateListSo.HealthGateList[0].activeGems)
        {

            if (gems != null){
            gems.isActive = false;
            }


        }
    }
    /*public void ResetLists()
    {
        foreach (var gems in healthGateListSo.HealthGateList[3].activeGems)
        {

            
             if (gems != null)
                {
                       int index = healthGateListSo.HealthGateList[3].activeGems.IndexOf(gems); 
                        Debug.Log(index);
                         healthGateListSo.HealthGateList[3].activeGems[index] = null;   
                }
           

        }
        foreach (var gems in healthGateListSo.HealthGateList[2].activeGems)
        {

             int index = healthGateListSo.HealthGateList[2].activeGems.IndexOf(gems);
              Debug.Log(index);
            healthGateListSo.HealthGateList[2].activeGems[index] = null;


        }
        foreach (var gems in healthGateListSo.HealthGateList[1].activeGems)
        {

            int index = healthGateListSo.HealthGateList[1].activeGems.IndexOf(gems);
             Debug.Log(index);
            healthGateListSo.HealthGateList[1].activeGems[index] = null;


        }
        foreach (var gems in healthGateListSo.HealthGateList[0].activeGems)
        {

             int index = healthGateListSo.HealthGateList[0].activeGems.IndexOf(gems);
              Debug.Log(index);
            healthGateListSo.HealthGateList[0].activeGems[index] = null;


        }
    }*/
    public void ResetBuffs()
    {
        _playerController.attackDamage = 10;
        _playerController.runSpeed = 4;
        _playerController.walkSpeed = 1.4f;
        _playerManager.defenceRate = 0;
        _playerController.lifeStealRate = 0;
        _playerController.jumpForce = 7f;
        _playerController.rollStaminaCost= 30f;
        Dagger.DaggerDamageDown();
        Physics2D.IgnoreLayerCollision (7, 8, false);
        _playerController.cooldownTime = 9f;
        _playerManager.shieldDefenceRate = 0.4f;
        _playerController.shieldStamina = 10f;
        _playerController.rollSeconds = 0.5f;
        _playerManager.regenHealth = 5f;
        _playerManager.isRegen = false;
    }
}


