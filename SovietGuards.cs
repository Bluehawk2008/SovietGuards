using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using MelonLoader;
using GHPC;
using GHPC.Mission;
using GHPC.State;
using GHPC.Vehicle;


namespace SovietGuards{

    public class AlreadyConverted : MonoBehaviour
    {
        void Awake()
        {
            enabled = false;
        }
    }
    public class SovietGuardsClass : MelonMod
    {
        public static GameObject gameManager;
        public static Material guards_mat = null;
        public static MelonPreferences_Entry<bool> hide_nets;
        public static MelonPreferences_Entry<bool> mute_logging;
        public static MelonPreferences_Entry<bool> BMP1s;
        public static MelonPreferences_Entry<bool> BMP2;
        public static MelonPreferences_Entry<bool> T62;
        public static MelonPreferences_Entry<bool> T64s;
        public static MelonPreferences_Entry<bool> T80;
        public static MelonPreferences_Entry<bool> BTR60;
        public static MelonPreferences_Entry<bool> BTR70;
        public static MelonPreferences_Entry<bool> BRDM;
        public static MelonPreferences_Entry<bool> Infantry;

        private bool menuProps = false;
        Vector2 newSize = new Vector2(8, 4);

        public override void OnInitializeMelon()
        {
            MelonPreferences_Category cfg = MelonPreferences.CreateCategory("SovietGuards");
            hide_nets = cfg.CreateEntry<bool>("Remove turret camo nets", false);
            hide_nets.Description = "Removes camo nets that might obscure the Guards emblem";

            mute_logging = cfg.CreateEntry<bool>("Mute console logging", false);
            mute_logging.Description = "Silences the mod's messages in the MelonLoader console";

            BMP1s = cfg.CreateEntry<bool>("Modify BMP1s", true);
            BMP1s.Description = "Set true to modify BMP1s, false to exclude them";
            BMP2 = cfg.CreateEntry<bool>("Modify BMP2s", true);
            BMP2.Description = "Set true to modify BMP2s, false to exclude them";
            T62 = cfg.CreateEntry<bool>("Modify T62s", true);
            T62.Description = "Set true to modify T62s, false to exclude them";
            T64s = cfg.CreateEntry<bool>("Modify T64s", true);
            T64s.Description = "Set true to modify T64s, false to exclude them";
            T80 = cfg.CreateEntry<bool>("Modify T80s", true);
            T80.Description = "Set true to modify T80s, false to exclude them";
            BTR60 = cfg.CreateEntry<bool>("Modify BTR60s", true);
            BTR60.Description = "Set true to modify BTR60s, false to exclude them";
            BTR70 = cfg.CreateEntry<bool>("Modify BTR70s", true);
            BTR70.Description = "Set true to modify BRDMs, false to exclude them";
            BRDM = cfg.CreateEntry<bool>("Modify BRDMs", true);
            BRDM.Description = "Set true to modify BRDMs, false to exclude them";
            Infantry = cfg.CreateEntry<bool>("Modify Infantry", true);
            Infantry.Description = "Set true to modify infantrymen, false to exclude them";            
        }        
        
        public void NewQuad(GameObject go, Material mat)
        {
            MeshFilter filter = go.AddComponent<MeshFilter>();
            MeshRenderer render = go.AddComponent<MeshRenderer>();
            filter.mesh = new Mesh();
            filter.mesh.vertices = new Vector3[] {
                            new Vector3(1f, 0 , 1f), new Vector3(1f, 0, -1f), new Vector3(-1f, 0, 1f), new Vector3(-1f, 0, -1f) };
            filter.mesh.uv = new Vector2[] {
                            new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 0) };
            filter.mesh.triangles = new int[] { 0, 1, 2, 2, 1, 3 };
            filter.mesh.RecalculateNormals();
            render.material = mat;
        }
        public void MenuProps()
        {
            //Since there is no unitspawner on the main menu scenes, we need to load in our dummy BMP the hard way
            
            UnitPrefabLookupScriptable lookup = Resources.FindObjectsOfTypeAll<UnitPrefabLookupScriptable>()[0];
            UnitPrefabLookupScriptable.UnitPrefabMetadata[] all_prefabs = lookup.AllUnits;

            foreach (var unit in all_prefabs)
            {
                if (unit.FriendlyName != "BMP-2") { continue; }
                unit.PrefabReference.LoadAssetAsync<GameObject>().WaitForCompletion();
                if (!mute_logging.Value) { MelonLogger.Msg("BMP-2 prefab loaded"); }
            }
            
            var list = Resources.FindObjectsOfTypeAll<Vehicle>();
            foreach (var vic in list)
            {
                if (vic.name == "BMP2 Soviet")
                {
                    MeshRenderer gvards_mr = vic.transform.Find("BMP2_markings_sa/GVARDS").gameObject.GetComponent<MeshRenderer>();
                    guards_mat = gvards_mr.material;
                    if (guards_mat == null) { MelonLogger.Msg("Can't retrieve guards material from prefab"); }
                    else { if (!mute_logging.Value) { MelonLogger.Msg("Got material from prefab"); } }                    
                }
            }            

            //since the prop vehicles in the scene have no 'Vehicle' component, and the BTR70 is not even tagged 'vehicle',
            //we fetch all the gameobjects in a big-ass array and filter them by their names
            GameObject[] props = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (var prop in props) {                
                if (prop.name.Length < 6) { continue; }
                string short_name = prop.name.Substring(0, 6);                
                switch (short_name)
                {
                    case "T64A S":
                        if (!T64s.Value) { continue; }
                        Transform tac1 = prop.transform.Find("---T64A_MESH---/HULL/TURRET/T64A_markings/tac marker");
                        Transform tac2 = prop.transform.Find("---T64A_MESH---/HULL/TURRET/luna_elbow_3/spotlight cover/tac marker001");
                        if (tac1 != null)
                        {
                            MeshRenderer markings1_mr = tac1.GetComponent<MeshRenderer>();
                            markings1_mr.material = guards_mat;
                            markings1_mr.material.mainTextureScale = newSize;
                        }
                        if (tac2 != null)
                        {
                            MeshRenderer markings2_mr = tac2.GetComponent<MeshRenderer>();
                            markings2_mr.material = guards_mat;
                            markings2_mr.material.mainTextureScale = newSize;
                        }
                        if (!mute_logging.Value) { MelonLogger.Msg("T64 prop inducted into the Guards!"); }
                        break;
                    case "T62 St":
                        if (!T62.Value) { continue; }
                        Transform markings = prop.transform.Find("---T62_rig---/HULL/TURRET/T62_markings/tac_markings");
                        MeshRenderer markings_mr = markings.GetComponent<MeshRenderer>();
                        markings_mr.material = guards_mat;                      
                        markings_mr.material.mainTextureScale = newSize;
                        if (hide_nets.Value)
                        {
                            GameObject net = prop.transform.Find("---T62_rig---/HULL/TURRET/T62 turret net").gameObject;
                            if (net != null) { net.SetActive(false); }
                        }
                        if (!mute_logging.Value) { MelonLogger.Msg("T62 prop inducted into the Guards!"); }
                        break;
                    case "T80B S":
                        if (!T80.Value) { continue; }
                        markings = prop.transform.Find("T80B_rig/HULL/TURRET/searchlight/searchlight cover/tac marker");
                        markings_mr = markings.GetComponent<MeshRenderer>();
                        markings_mr.material = guards_mat;
                        markings_mr.material.mainTextureScale = newSize;
                        if (!mute_logging.Value) { MelonLogger.Msg("T80 prop inducted into the Guards!"); }
                        break;
                    case "BMP2 S":
                        if (!BMP2.Value) { continue; }
                        prop.transform.Find("BMP2_markings_sa/GVARDS").gameObject.SetActive(true);
                        prop.transform.Find("BMP2_rig/HULL/TURRET/tactical marker").gameObject.SetActive(false);
                        prop.transform.Find("BMP2_rig/HULL/TURRET/GVARDS").localScale = new Vector3(1.005f, 1f, 1f);
                        if (hide_nets.Value)
                        {
                            GameObject net = prop.transform.Find("BMP2_rig/HULL/TURRET/bmp2 net turret").gameObject;
                            if (net != null) { net.SetActive(false); }
                        }
                        if (!mute_logging.Value) { MelonLogger.Msg("BMP2 prop inducted into the Guards!"); }
                        break;
                    case "BMP1P ":
                        if (!BMP1s.Value) { continue; }
                        markings = prop.transform.Find("BMP1_rig/HULL/TURRET/tac marker");
                        markings_mr = markings.GetComponent<MeshRenderer>();
                        markings_mr.material = guards_mat;
                        markings_mr.material.mainTextureScale = newSize;
                        if (hide_nets.Value)
                        {
                            GameObject net = prop.transform.Find("BMP1_rig/HULL/TURRET/bmp1 net turret").gameObject;
                            if (net != null) { net.SetActive(false); }
                        }
                        if (!mute_logging.Value) { MelonLogger.Msg("BMP1P prop inducted into the Guards!"); }
                        break;
                    case "BTR70 ":
                        if (!BTR70.Value) { continue; }
                        GameObject turret = prop.transform.Find("BTR70_rig/HULL/TURRET").gameObject;
                        GameObject guard_left = new GameObject("Guards_left");
                        guard_left.transform.parent = turret.transform;                        
                        guard_left.transform.position = turret.transform.position;
                        NewQuad(guard_left, guards_mat);
                        guard_left.transform.localScale = new Vector3(14f, 14f, 14f);
                        guard_left.transform.localPosition += new Vector3(-56.55f, 5.0f, -14.0f);
                        guard_left.transform.rotation = turret.transform.rotation * Quaternion.Euler(new Vector3(309.0f, 70.0f, 5.1f));                        

                        GameObject guard_right = new GameObject("Guards_right");
                        guard_right.transform.parent = turret.transform;                        
                        guard_right.transform.position = turret.transform.position;
                        NewQuad(guard_right, guards_mat);
                        guard_right.transform.localScale = new Vector3(14f, 14f, 14f);
                        guard_right.transform.localPosition += new Vector3(57.55f, 5.0f, -14.0f);
                        guard_right.transform.rotation = turret.transform.rotation * Quaternion.Euler(new Vector3(309.0f, -90.0f, 5.1f));                        

                        if (!mute_logging.Value) { MelonLogger.Msg("BTR70 prop inducted into the Guards!"); }
                        break;
                }
            }
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            guards_mat = null;
            if (sceneName == "MainMenu2_Scene" || sceneName == "t64_menu" || sceneName == "MainMenu2-1_Scene")
            {
                menuProps = true;
                MenuProps();
            }
            else { menuProps = false; }            
            
            gameManager = GameObject.Find("_APP_GHPC_");
            if (gameManager == null) return;

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(Conversion), GameStatePriority.Medium);
        }
        private IEnumerator Conversion(GameState _)
        {
            if (menuProps == true) { yield break; }                       
            Vehicle[] list = GameObject.FindObjectsByType<Vehicle>(FindObjectsSortMode.None);

            foreach (var unit in list)
            {
                GameObject unit_go = unit.gameObject;
                if (unit_go == null) { continue; }
                if (unit_go.GetComponent<AlreadyConverted>() != null) { continue; }
                if (unit.UniqueName != "BMP2_SA") { continue; }

                GameObject gvards = unit_go.transform.Find("BMP2_markings_sa/GVARDS").gameObject;
                if (gvards != null)
                {
                    guards_mat = gvards.GetComponent<MeshRenderer>().material; //ref for Guard badge texture
                    gvards.transform.localScale = new Vector3(1.005f, 1f, 1f); //slight resize prevents clipping
                    if (BMP2.Value) 
                    { 
                        gvards.SetActive(true);
                        unit.transform.Find("BMP2_rig/HULL/TURRET/tactical marker").gameObject.SetActive(false);
                    } //switches on BMP2's Guard badge
                    
                    if (!mute_logging.Value) { MelonLogger.Msg("Found BMP-2 named " + unit + " and its guard badge: " + guards_mat); }
                    
                    if (hide_nets.Value) { 
                        GameObject net = unit.transform.Find("BMP2_rig/HULL/TURRET/bmp2 net turret").gameObject;
                        if (net != null) { net.SetActive(false); }
                    }                    
                    unit_go.AddComponent<AlreadyConverted>();
                }
            }

            if (guards_mat == null) { //if there's no BMP-2 in the scene, we create one and take its Guard badge
                var prefabLookups = UnityEngine.Object.FindAnyObjectByType<UnitSpawner>().PrefabLookup;
                AssetReference prefab = prefabLookups.GetPrefab("BMP2_SA");
                var dummy_bmp = Addressables.LoadAssetAsync<GameObject>(prefab).WaitForCompletion();
                MelonLogger.Msg(dummy_bmp.name + "vehicle object");

                if (dummy_bmp == null) {                     
                    if (!mute_logging.Value) { MelonLogger.Msg("Could not load a BMP-2 prefab!"); }
                }
                else {
                    MelonLogger.Msg(dummy_bmp);                    
                    MeshRenderer gvards_mr = dummy_bmp.transform.Find("BMP2_markings_sa/GVARDS").gameObject.GetComponent<MeshRenderer>();
                    guards_mat = gvards_mr.material;
                    if (guards_mat == null) { MelonLogger.Msg("Can't retrieve guards material from dummy"); }
                    else { if (!mute_logging.Value) { MelonLogger.Msg("Got " + guards_mat + "from dummy"); } } 
                }
            }
                
            foreach (var unit in list)
            {
                GameObject unit_go = unit.gameObject;
                if (unit_go == null) { continue; }
                if (unit_go.GetComponent<AlreadyConverted>() != null) { continue; }
                switch (unit.UniqueName) 
                {
                    case ("T62"):
                        if (!T62.Value) { continue; }
                        Transform markings = unit_go.transform.Find("---T62_rig---/HULL/TURRET/T62_markings/tac_markings");
                        MeshRenderer markings_mr = markings.GetComponent<MeshRenderer>();
                        markings_mr.material = guards_mat; //replaces tactical markings with Guard badge                       
                        markings_mr.material.mainTextureScale = newSize; //new material needs adjusting to fit mesh
                        if (!mute_logging.Value) { MelonLogger.Msg("T-62 named " + unit + " inducted into the Guards!"); }
                        if (hide_nets.Value)
                        {
                            GameObject net = unit.transform.Find("---T62_rig---/HULL/TURRET/T62 turret net").gameObject;
                            if (net != null) { net.SetActive(false); }
                        }
                        unit_go.AddComponent<AlreadyConverted>();
                        break;
                    case ("T64A"):   
                    case ("T64B"):                    
                    case ("T64A74"):                    
                    case ("T64A81"): 
                    case ("T64A83"):
                    case ("T64A84"):
                    case ("T64B81"):
                    case ("T64B1"):
                    case ("T64B181"):
                        if (!T64s.Value) { continue; }
                        Transform markings1 = unit_go.transform.Find("---T64A_MESH---/HULL/TURRET/T64A_markings/tac marker");
                        Transform markings2 = unit_go.transform.Find("---T64A_MESH---/HULL/TURRET/luna_elbow_3/spotlight cover/tac marker001");
                        if (markings1 != null) { 
                            MeshRenderer markings1_mr = markings1.GetComponent<MeshRenderer>();
                            markings1_mr.material = guards_mat;
                            markings1_mr.material.mainTextureScale = newSize;
                        }
                        if (markings2 != null) { 
                            MeshRenderer markings2_mr = markings2.GetComponent<MeshRenderer>();
                            markings2_mr.material = guards_mat;
                            markings2_mr.material.mainTextureScale = newSize;
                        }
                        if (!mute_logging.Value) { MelonLogger.Msg("T-64 named " + unit + " inducted into the Guards!"); }
                        unit_go.AddComponent<AlreadyConverted>();
                        break;
                    case ("T80B"):
                        if (!T80.Value) { continue; }
                        markings = unit_go.transform.Find("T80B_rig/HULL/TURRET/searchlight/searchlight cover/tac marker");
                        markings_mr = markings.GetComponent<MeshRenderer>();
                        markings_mr.material = guards_mat;
                        markings_mr.material.mainTextureScale = newSize;
                        if (!mute_logging.Value) { MelonLogger.Msg("T-80 named " + unit + " inducted into the Guards!"); }
                        unit_go.AddComponent<AlreadyConverted>();
                        break;
                    case ("BMP1_SA"):
                    case ("BMP1P_SA"):
                        if (!BMP1s.Value) { continue; }
                        markings = unit_go.transform.Find("BMP1_rig/HULL/TURRET/tac marker");
                        markings_mr = markings.GetComponent<MeshRenderer>();
                        markings_mr.material = guards_mat;
                        markings_mr.material.mainTextureScale = newSize;
                        if (!mute_logging.Value) { MelonLogger.Msg("BMP-1P named " + unit + "inducted into the Guards!"); }
                        if (hide_nets.Value)
                        {
                            GameObject net = unit.transform.Find("BMP1_rig/HULL/TURRET/bmp1 net turret").gameObject;
                            if (net != null) { net.SetActive(false); }
                        }
                        unit_go.AddComponent<AlreadyConverted>();
                        break;
                    case ("BTR60PB_SA"):                    
                        if (!BTR60.Value) { continue; }                        
                        unit_go.transform.Find("BTR_nva_markings/Object266").gameObject.SetActive(true);                          
                        markings = unit_go.transform.Find("btr60_rig/HULL/TURRET/Object266");
                        markings_mr = markings.GetComponent<MeshRenderer>();
                        markings_mr.material = guards_mat;
                        markings_mr.material.mainTextureScale = new Vector2(1.05f, 1.05f); //overwriting the DDR rondel requires a unique rescale
                        if (!mute_logging.Value) { MelonLogger.Msg("BTR-60 named " + unit + "inducted into the Guards!"); }
                        unit_go.AddComponent<AlreadyConverted>();
                        break;
                    case ("BRDM2_SA"):
                        if (!BRDM.Value) { continue; }
                        unit_go.transform.Find("BRDM2_numbers/emblem").gameObject.SetActive(true);
                        markings = unit_go.transform.Find("BRDM2_rig/HULL/TURRET/emblem");
                        markings_mr = markings.GetComponent<MeshRenderer>();
                        markings_mr.material = guards_mat;
                        markings_mr.material.mainTextureScale = new Vector2(1.35f, 1.35f);
                        markings_mr.material.mainTextureOffset = new Vector2(-0.1f, -0.16f);
                        if (!mute_logging.Value) { MelonLogger.Msg("BRDM named " + unit + "inducted into the Guards!"); }
                        unit_go.AddComponent<AlreadyConverted>();
                        break;
                    case ("BTR70"): //BTR70 has no tactical symbols to overwrite, so we need to create new planes from scratch!                    
                        if (!BTR70.Value) { continue; }                        
                        GameObject turret;
                        turret = unit.transform.Find("BTR70_rig/HULL/TURRET").gameObject;  
                        GameObject guard_left = new GameObject("Guards_left");
                        guard_left.transform.parent = turret.transform;                        
                        guard_left.transform.position = turret.transform.position;
                        NewQuad(guard_left, guards_mat);
                        guard_left.transform.localScale = new Vector3(14f, 14f, 14f);
                        guard_left.transform.localPosition += new Vector3(-56.55f, 5.0f, -14.0f);
                        guard_left.transform.rotation = turret.transform.rotation * Quaternion.Euler(new Vector3(309.0f, 70.0f, 5.1f)); 

                        GameObject guard_right = new GameObject("Guards_right");
                        guard_right.transform.parent = turret.transform;                        
                        guard_right.transform.position = turret.transform.position;
                        NewQuad(guard_right, guards_mat);
                        guard_right.transform.localScale = new Vector3(14f, 14f, 14f);
                        guard_right.transform.localPosition += new Vector3(57.55f, 5.0f, -14.0f);
                        guard_right.transform.rotation = turret.transform.rotation * Quaternion.Euler(new Vector3(309.0f, -90.0f, 5.1f)); 

                        if (!mute_logging.Value) { MelonLogger.Msg("BTR-70 named " + unit + "inducted into the Guards!"); }
                        unit_go.AddComponent<AlreadyConverted>();
                        break;
                    case ("T64R"):
                        if (!T64s.Value) { continue; }
                        unit.transform.Find("T64A_markings").gameObject.SetActive(true);
                        unit.transform.Find("---T64A_MESH---").gameObject.SetActive(true);
                        unit.transform.Find("---T64A_MESH---/HULL/TURRET/T64A_markings").gameObject.SetActive(true);
                        GameObject tac_luna = unit.transform.Find("---T64A_MESH---/HULL/TURRET/luna_elbow_3/tac marker001").gameObject;
                        tac_luna.transform.SetParent(unit.transform.Find("T64R_rig/HULL/TURRET/luna_elbow_3/luna cover"), true);
                        tac_luna.transform.localPosition = new Vector3(0f, 0f, 0f);
                        MeshRenderer tac_luna_mr = tac_luna.GetComponent<MeshRenderer>();
                        tac_luna_mr.material = guards_mat;
                        tac_luna_mr.material.mainTextureScale = newSize;
                        unit.transform.Find("---T64A_MESH---").gameObject.SetActive(false);
                        if (!mute_logging.Value) { MelonLogger.Msg("T-64 named " + unit + "inducted into the Guards!"); }
                        unit_go.AddComponent<AlreadyConverted>();
                        break;
                } 
            }

            //Now for the infantry
            if (!Infantry.Value) { yield break; }
            Unit[] units = GameObject.FindObjectsByType<Unit>(FindObjectsSortMode.None);
            foreach (var unit in units)
            {
                GameObject unit_go = unit.gameObject;
                if (unit_go == null) { continue; }
                if (unit_go.GetComponent<AlreadyConverted>() != null) { continue; }
                if (unit.name.StartsWith("SA Obr73")){                    
                    GameObject torso = unit.transform.Find("Troop Base/RED_OBR73_KHAKI/dress").gameObject;
                    Transform[] bones = torso.GetComponent<SkinnedMeshRenderer>().bones;
                    Transform chest = null;
                    foreach (var bone in bones)
                    {
                        if (bone.name == "soldierChest") { chest = bone; }                        
                    }                    
                    GameObject guard = new GameObject("Guards_badge");
                    guard.transform.parent = chest;                    
                    guard.transform.position = chest.transform.position;
                    NewQuad(guard, guards_mat);
                    guard.transform.localScale = new Vector3(0.023f, 0.023f, 0.023f);
                    guard.transform.localPosition += new Vector3(-0.03f, 0.13f, -0.07f);
                    guard.transform.rotation = chest.transform.rotation * Quaternion.Euler(new Vector3(20f, -90f, 5f));
                    if (!mute_logging.Value) { MelonLogger.Msg("Motostrelok inducted into the Guards!"); }
                    unit_go.AddComponent<AlreadyConverted>();
                }
            }
        }        
    }
}
