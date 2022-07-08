using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMech : MonoBehaviour
{

    // Use Scriptable Objects instead of Scripts to refrence the componet data?
    //Deal with Riser


    [SerializeField] List<GameObject> Chassis;
    [SerializeField] List<GameObject> Conectors;
    [SerializeField] List<GameObject> Cockpits;
    [SerializeField] List<GameObject> Backpacks;
    [SerializeField] List<GameObject> Shoulders;
    [SerializeField] List<GameObject> Weapons;

    GameObject previousMech;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GenerateNewMech();
        }
    }

    void GenerateNewMech()
    {
        if (previousMech != null)
            Destroy(previousMech);

        //Spawn Chassi
        GameObject randomChassi = GetRandomPart(Chassis);
        GameObject insChassi = Instantiate(randomChassi, Vector3.zero, Quaternion.identity);
        //Find attached Spawn Points
        MechChassi mechChassi = insChassi.GetComponent<MechChassi>();
        Transform cockpitSpawnPoint = mechChassi.Mount_Top.transform;

        //Spawn Cockpit
        GameObject randomCockpit = GetRandomPart(Cockpits);
        GameObject insCockpit = Instantiate(randomCockpit,cockpitSpawnPoint.position, Quaternion.identity);
        insCockpit.transform.parent = cockpitSpawnPoint;
        //Find attached Spawn Points
        MechCockpit mechCockpit = insCockpit.GetComponent<MechCockpit>();
        Transform BackpackSpawnPoint = mechCockpit.Mount_Backpack;
        Transform LeftWeaponSpawnPoint = mechCockpit.Mount_Weapon_Left;
        Transform RightWeaponSpawnPoint = mechCockpit.Mount_Weapon_Right;
        Transform TopWeaponSpawnPoint = mechCockpit.Mount_Top;

        // if (mechCockpit.cockpitTypes == MechCockpit.CockpitTypes.Riser)
        // {
        //     Debug.Log("Riser");
        //     GameObject randomSecondCockpit = GetRandomPart(Cockpits);
        //     GameObject insSecondCockpit = Instantiate(randomSecondCockpit, TopWeaponSpawnPoint.position, Quaternion.identity);
        //     insSecondCockpit.transform.parent = TopWeaponSpawnPoint;
        //     //Find attached Spawn Points
        //     MechCockpit mechSecondCockpit = insSecondCockpit.GetComponent<MechCockpit>();
        //     BackpackSpawnPoint = mechSecondCockpit.Mount_Backpack;
        //     LeftWeaponSpawnPoint = mechSecondCockpit.Mount_Weapon_Left;
        //     RightWeaponSpawnPoint = mechSecondCockpit.Mount_Weapon_Right;
        //     TopWeaponSpawnPoint = mechSecondCockpit.Mount_Top;
        // }

        //Spawn Backpack
        GameObject randomBackpack = GetRandomPart(Backpacks);
        GameObject insBackpack = Instantiate(randomBackpack, BackpackSpawnPoint.position, Quaternion.identity);
        insBackpack.transform.parent = BackpackSpawnPoint;
        //Find attached Spawn Points
        MechBackpack mechBackpack = insBackpack.GetComponent<MechBackpack>();
        Transform AntennaSpawnPoint = mechBackpack.Mount_Antenna;
        Transform WheelSpawnPoint = mechBackpack.Mount_Wheel_Top;
        Transform TopGunSpawnPoint = mechBackpack.Mount_Weapon_Top;

        //Spawn LeftWeapon
        GameObject randomLeftWeapon = GetRandomPart(Weapons);
        GameObject insLeftWeapon = Instantiate(randomLeftWeapon, LeftWeaponSpawnPoint.position, Quaternion.identity);
        insLeftWeapon.transform.parent = LeftWeaponSpawnPoint;
        // Find attached weapons info
        MechWeapon mechLeftWeapon = insLeftWeapon.GetComponent<MechWeapon>();

        //Spawn RightWeapon
        GameObject randomRightWeapon = GetRandomPart(Weapons);
        GameObject insRightWeapon = Instantiate(randomRightWeapon, RightWeaponSpawnPoint.position, Quaternion.Euler(new Vector3(0,0,180)));
        insRightWeapon.transform.parent = RightWeaponSpawnPoint;
        // Find attached weapons info
        MechWeapon mechRightWeapon = insRightWeapon.GetComponent<MechWeapon>();

        //Spawn ToptWeapon
        GameObject randomTopWeapon = GetRandomPart(Weapons);
        GameObject insTopWeapon = Instantiate(randomTopWeapon, TopWeaponSpawnPoint.position, Quaternion.Euler(new Vector3(0, 0, -90)));
        insTopWeapon.transform.parent = TopWeaponSpawnPoint;
        // Find attached weapons info
        MechWeapon mechTopWeapon = insTopWeapon.GetComponent<MechWeapon>();

        //Spawn Antina

        //Spawn Wheel

        //Spawn Backpack Gun

        //what about the Riser and Top_Weapon_Platform dont fit current modle? maybe this can be emplimented in the menu system as being attachaible to top of chassie weapon/ top of backpack weapon

        //what about Shoulder stuff that I dont know where to put?

        previousMech = insChassi;
    }

    void SpawnMechParts(List<GameObject> parts, Transform socket) // old spawn code for simple gun, doesnt work when the sub parts have span points too
    {
        GameObject randomPart = GetRandomPart(parts);
        GameObject insPart = Instantiate(randomPart, socket.transform.position, socket.transform.rotation);
        insPart.transform.parent = socket;
    }   

    GameObject GetRandomPart(List<GameObject> partsList)
    {
        int randomNumber = Random.Range(0, partsList.Count);
        return partsList[randomNumber];
    }
    
}
