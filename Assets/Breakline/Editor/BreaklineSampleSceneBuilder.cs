using System;
using System.Collections.Generic;
using System.Reflection;
using Breakline.Runtime.Hud;
using Breakline.Runtime.Match;
using Breakline.Runtime.Players;
using Breakline.Runtime.Tiles;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Breakline.Editor
{
    public static class BreaklineSampleSceneBuilder
    {
        private const string SampleScenePath = "Assets/Scenes/SampleScene.unity";
        private const string RootName = "BreaklinePrototype";

        [MenuItem("Tools/Breakline/Build Sample Scene")]
        public static void BuildSampleScene()
        {
            var scene = EditorSceneManager.OpenScene(SampleScenePath, UnityEditor.SceneManagement.OpenSceneMode.Single);
            var existingRoot = GameObject.Find(RootName);
            if (existingRoot != null)
            {
                Object.DestroyImmediate(existingRoot);
            }

            EnsureCamera();

            var root = new GameObject(RootName);

            var arenaRoot = new GameObject("ArenaRoot");
            arenaRoot.transform.SetParent(root.transform);

            var tileManagerObject = new GameObject("TileManager");
            tileManagerObject.transform.SetParent(arenaRoot.transform);
            var tileManager = tileManagerObject.AddComponent<DestructibleTileManager>();

            BuildTileField(arenaRoot.transform);
            tileManager.RefreshTileList();

            var matchRoot = new GameObject("MatchRoot");
            matchRoot.transform.SetParent(root.transform);
            var hudBridge = matchRoot.AddComponent<BreaklineHudBridge>();
            var matchTimer = matchRoot.AddComponent<MatchTimer>();
            var scoreTracker = matchRoot.AddComponent<ScoreTracker>();
            var ringOut = CreateRingOutZone(root.transform);
            var localTrophyProgress = matchRoot.AddComponent<LocalTrophyProgress>();
            var matchState = matchRoot.AddComponent<MatchStateController>();

            SetField(matchTimer, "durationSeconds", 120f);
            SetField(matchTimer, "playOnStart", false);
            SetField(localTrophyProgress, "startingTrophies", 0);
            SetField(localTrophyProgress, "enableWinStreakBonus", false);

            var spawnP1 = CreateSpawn(root.transform, "Spawn_P1", new Vector3(-2.5f, 1.2f, 0f));
            var spawnP2 = CreateSpawn(root.transform, "Spawn_P2", new Vector3(2.5f, 1.2f, 0f));

            var playerOne = CreatePlayer(
                root.transform,
                "Player_1",
                0,
                new Vector3(-2.5f, 1.2f, 0f),
                new Color(0.18f, 0.85f, 0.95f),
                useBot: false,
                target: null);

            var playerTwo = CreatePlayer(
                root.transform,
                "Player_2_Bot",
                1,
                new Vector3(2.5f, 1.2f, 0f),
                new Color(1f, 0.45f, 0.3f),
                useBot: true,
                target: playerOne);

            var bot = playerTwo.GetComponent<BotController>();
            if (bot != null)
            {
                SetField(bot, "target", playerOne);
            }

            ConfigureMatchState(matchState, matchTimer, scoreTracker, ringOut, hudBridge, localTrophyProgress, playerOne, playerTwo, spawnP1, spawnP2);
            BuildHud(root.transform, hudBridge, playerOne, tileManager);

            EditorSceneManager.MarkSceneDirty(scene);
            Selection.activeObject = root;
        }

        private static void EnsureCamera()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                camera = cameraObject.AddComponent<Camera>();
                camera.tag = "MainCamera";
            }

            camera.orthographic = true;
            camera.orthographicSize = 6f;
            camera.transform.position = new Vector3(0f, 1.5f, -10f);
            camera.backgroundColor = new Color(0.06f, 0.08f, 0.12f, 1f);
            camera.clearFlags = CameraClearFlags.SolidColor;
        }

        private static RingOutDetector CreateRingOutZone(Transform parent)
        {
            var zone = new GameObject("RingOutZone");
            zone.transform.SetParent(parent);
            zone.transform.position = new Vector3(0f, -4.5f, 0f);

            var collider = zone.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(30f, 2f);

            return zone.AddComponent<RingOutDetector>();
        }

        private static Transform CreateSpawn(Transform parent, string name, Vector3 position)
        {
            var spawn = new GameObject(name);
            spawn.transform.SetParent(parent);
            spawn.transform.position = position;
            return spawn.transform;
        }

        private static PlayerController CreatePlayer(Transform parent, string name, int playerIndex, Vector3 position, Color color, bool useBot, PlayerController target)
        {
            var player = new GameObject(name);
            player.transform.SetParent(parent);
            player.transform.position = position;

            var spriteRenderer = player.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = GetBuiltInSprite();
            spriteRenderer.color = color;
            player.transform.localScale = new Vector3(0.9f, 1.1f, 1f);

            var rigidbody = player.AddComponent<Rigidbody2D>();
            rigidbody.gravityScale = 3f;
            rigidbody.freezeRotation = true;

            player.AddComponent<BoxCollider2D>().size = new Vector2(0.8f, 1f);

            var controller = player.AddComponent<PlayerController>();
            SetField(controller, "playerIndex", playerIndex);
            SetField(controller, "readKeyboardInput", !useBot);

            if (!useBot)
            {
                if (playerIndex == 0)
                {
                    SetField(controller, "moveLeftKey", UnityEngine.InputSystem.Key.A);
                    SetField(controller, "moveRightKey", UnityEngine.InputSystem.Key.D);
                    SetField(controller, "dashKey", UnityEngine.InputSystem.Key.LeftShift);
                    SetField(controller, "attackKey", UnityEngine.InputSystem.Key.Space);
                }
                else
                {
                    SetField(controller, "moveLeftKey", UnityEngine.InputSystem.Key.LeftArrow);
                    SetField(controller, "moveRightKey", UnityEngine.InputSystem.Key.RightArrow);
                    SetField(controller, "dashKey", UnityEngine.InputSystem.Key.RightShift);
                    SetField(controller, "attackKey", UnityEngine.InputSystem.Key.RightCtrl);
                }
            }

            var hurtboxObject = new GameObject("Hurtbox");
            hurtboxObject.transform.SetParent(player.transform);
            hurtboxObject.transform.localPosition = Vector3.zero;
            var hurtboxCollider = hurtboxObject.AddComponent<BoxCollider2D>();
            hurtboxCollider.isTrigger = true;
            hurtboxCollider.size = new Vector2(0.9f, 1.1f);
            var hurtbox = hurtboxObject.AddComponent<Breakline.Runtime.Combat.Hurtbox>();
            SetField(hurtbox, "owner", controller);

            var attackObject = new GameObject("AttackHitbox");
            attackObject.transform.SetParent(player.transform);
            attackObject.transform.localPosition = new Vector3(0f, 0.1f, 0f);
            var attackHitbox = attackObject.AddComponent<Breakline.Runtime.Combat.AttackHitbox>();
            SetField(controller, "attackHitbox", attackHitbox);

            if (useBot)
            {
                var bot = player.AddComponent<BotController>();
                SetField(bot, "self", controller);
                SetField(bot, "selfBody", rigidbody);
                SetField(bot, "target", target);
            }

            return controller;
        }

        private static void BuildTileField(Transform parent)
        {
            var builtInSprite = GetBuiltInSprite();
            var tileRows = 3;
            var tileColumns = 12;
            var tileWidth = 1f;
            var tileHeight = 0.55f;
            var startX = -5.5f;
            var startY = -0.5f;

            for (var y = 0; y < tileRows; y++)
            {
                for (var x = 0; x < tileColumns; x++)
                {
                    var tile = new GameObject($"Tile_{x}_{y}");
                    tile.transform.SetParent(parent);
                    tile.transform.position = new Vector3(startX + x * tileWidth, startY + y * tileHeight, 0f);

                    var support = tile.AddComponent<BoxCollider2D>();
                    support.size = new Vector2(0.98f, 0.5f);

                    var destructibleTile = tile.AddComponent<DestructibleTile>();

                    var intact = CreateTileVisual(tile.transform, "IntactVisual", builtInSprite, new Color(0.90f, 0.93f, 0.97f));
                    var cracked = CreateTileVisual(tile.transform, "CrackedVisual", builtInSprite, new Color(0.96f, 0.78f, 0.35f));
                    var broken = CreateTileVisual(tile.transform, "BrokenVisual", builtInSprite, new Color(0.12f, 0.12f, 0.14f));

                    SetField(destructibleTile, "maxDurability", 2);
                    SetField(destructibleTile, "damageLockoutSeconds", 0.05f);
                    SetField(destructibleTile, "crackedVisualHoldSeconds", 0f);
                    SetField(destructibleTile, "brokenAutoRestoreSeconds", -1f);
                    SetField(destructibleTile, "disableCollisionWhenBroken", true);
                    SetField(destructibleTile, "supportColliders", new[] { support });
                    SetField(destructibleTile, "intactVisualRoot", intact);
                    SetField(destructibleTile, "crackedVisualRoot", cracked);
                    SetField(destructibleTile, "brokenVisualRoot", broken);
                }
            }
        }

        private static GameObject CreateTileVisual(Transform parent, string name, Sprite sprite, Color color)
        {
            var visual = new GameObject(name);
            visual.transform.SetParent(parent);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = new Vector3(0.98f, 0.5f, 1f);

            var renderer = visual.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            return visual;
        }

        private static void ConfigureMatchState(
            MatchStateController matchState,
            MatchTimer matchTimer,
            ScoreTracker scoreTracker,
            RingOutDetector ringOut,
            BreaklineHudBridge hudBridge,
            LocalTrophyProgress localTrophyProgress,
            PlayerController playerOne,
            PlayerController playerTwo,
            Transform spawnOne,
            Transform spawnTwo)
        {
            SetField(matchState, "matchTimer", matchTimer);
            SetField(matchState, "scoreTracker", scoreTracker);
            SetField(matchState, "ringOutDetector", ringOut);
            SetField(matchState, "hudBridge", hudBridge);
            SetField(matchState, "localTrophyProgress", localTrophyProgress);
            SetField(matchState, "players", new List<PlayerController> { playerOne, playerTwo });
            SetField(matchState, "spawnPoints", new List<Transform> { spawnOne, spawnTwo });
            SetField(matchState, "respawnDelaySeconds", 1.1f);
        }

        private static void BuildHud(Transform parent, BreaklineHudBridge hudBridge, PlayerController localPlayer, DestructibleTileManager tileManager)
        {
            var canvasObject = new GameObject("Canvas");
            canvasObject.transform.SetParent(parent);
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObject.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920f, 1080f);
            canvasObject.AddComponent<GraphicRaycaster>();

            var hudController = canvasObject.AddComponent<BreaklineHudController>();

            var timerText = CreateText(canvasObject.transform, "TimerText", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -70f), 54, TextAnchor.MiddleCenter, "02:00");
            var playerScoreText = CreateText(canvasObject.transform, "PlayerScoreText", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(120f, -70f), 48, TextAnchor.MiddleLeft, "0");
            var enemyScoreText = CreateText(canvasObject.transform, "EnemyScoreText", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-120f, -70f), 48, TextAnchor.MiddleRight, "0");

            var bannerRoot = CreatePanel(canvasObject.transform, "ResultBanner", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(520f, 120f), new Color(0f, 0f, 0f, 0.75f));
            bannerRoot.SetActive(false);
            var bannerText = CreateText(bannerRoot.transform, "ResultBannerText", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, 52, TextAnchor.MiddleCenter, "YOU WIN");

            var cooldownRoot = CreatePanel(canvasObject.transform, "CooldownPanel", new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(170f, 110f), new Vector2(280f, 120f), new Color(0f, 0f, 0f, 0.45f));
            var dashFill = CreateFillImage(cooldownRoot.transform, "DashFill", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -28f), new Vector2(220f, 18f), new Color(0.15f, 0.8f, 1f, 1f));
            var attackFill = CreateFillImage(cooldownRoot.transform, "AttackFill", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -76f), new Vector2(220f, 18f), new Color(1f, 0.4f, 0.25f, 1f));
            var dashLabel = CreateText(cooldownRoot.transform, "DashLabel", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -26f), 24, TextAnchor.MiddleLeft, "DASH");
            var attackLabel = CreateText(cooldownRoot.transform, "AttackLabel", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -74f), 24, TextAnchor.MiddleLeft, "ATK");

            var tileDebugText = CreateText(canvasObject.transform, "TileDebugText", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 40f), 24, TextAnchor.MiddleCenter, "Tiles");

            var cooldownHud = cooldownRoot.AddComponent<PlayerCooldownHudController>();
            var tileDebugHud = tileDebugText.gameObject.AddComponent<TileDebugHudController>();

            SetField(hudController, "hudBridge", hudBridge);
            SetField(hudController, "timerText", timerText);
            SetField(hudController, "playerScoreText", playerScoreText);
            SetField(hudController, "enemyScoreText", enemyScoreText);
            SetField(hudController, "resultBannerRoot", bannerRoot);
            SetField(hudController, "resultBannerText", bannerText);

            SetField(cooldownHud, "player", localPlayer);
            SetField(cooldownHud, "dashCooldownFill", dashFill);
            SetField(cooldownHud, "attackCooldownFill", attackFill);
            SetField(cooldownHud, "dashLabel", dashLabel);
            SetField(cooldownHud, "attackLabel", attackLabel);

            SetField(tileDebugHud, "tileManager", tileManager);
            SetField(tileDebugHud, "tileStateText", tileDebugText);
        }

        private static Text CreateText(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, int fontSize, TextAnchor alignment, string initialText)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent);
            var rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(500f, 80f);

            var text = textObject.AddComponent<Text>();
            text.text = initialText;
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = alignment;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return text;
        }

        private static GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 size, Color color)
        {
            var panel = new GameObject(name);
            panel.transform.SetParent(parent);
            var rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            var image = panel.AddComponent<Image>();
            image.color = color;
            return panel;
        }

        private static Image CreateFillImage(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 size, Color color)
        {
            var imageObject = new GameObject(name);
            imageObject.transform.SetParent(parent);
            var rect = imageObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            var image = imageObject.AddComponent<Image>();
            image.color = color;
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillOrigin = 0;
            image.fillAmount = 1f;
            return image;
        }

        private static Sprite GetBuiltInSprite()
        {
            return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        }

        private static void SetField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                throw new MissingFieldException(target.GetType().Name, fieldName);
            }

            field.SetValue(target, value);
            EditorUtility.SetDirty((Object)target);
        }
    }
}
