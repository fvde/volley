using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ManhattanMorning.Misc;
using ManhattanMorning.Misc.Tasks;
using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Model;
using ManhattanMorning.View;

using Microsoft.Xna.Framework;

namespace ManhattanMorning.Controller
{
    /// <summary>
    /// PowerUpManager is responsible for
    /// the creation and management of PowerUps
    /// </summary>
    class PowerUpManager : IController
    {

        #region Properties

        /// <summary>
        /// Implementation of Singleton Pattern
        /// </summary>
        public static PowerUpManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PowerUpManager();
                }
                return instance;
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// Implementation of Singleton Pattern
        /// </summary>
        private static PowerUpManager instance;

        /// <summary>
        /// Random number generator.
        /// </summary>
        private Random randomizer;

        /// <summary>
        /// Includes all the types of powerups that are allowed in this level.
        /// </summary>
        private List<PowerUpType> allowedPowerUps;

        /// <summary>
        /// Includes all the types of positive powerUps.
        /// </summary>
        private List<PowerUpType> positivePowerUps;

        /// <summary>
        /// Includes all the types of positive powerUps.
        /// </summary>
        private List<PowerUpType> neutralPowerUps;

        /// <summary>
        /// State of the Controller. Paused or unpaused.
        /// </summary>
        private bool paused;

        /// <summary>
        /// All gameObjects that are currently in the game.
        /// </summary>
        private LayerList<LayerInterface> gameObjects;

        /// <summary>
        /// All powerups that are active at the moment.
        /// </summary>
        private List<PowerUp> activePowerUps;

        /// <summary>
        /// instance of the Settingsmanager
        /// </summary>
        private SettingsManager settingsManager;


        #endregion


        #region Initialization

        /// <summary>
        /// Initializes PowerUpManager
        /// hidden because of Singleton Pattern
        /// </summary>
        private PowerUpManager()
        {
            allowedPowerUps = new List<PowerUpType>();

            // Set a seed here
            randomizer = new Random();
            paused = false;

            // SettingsManager
            settingsManager = SettingsManager.Instance;

            setPowerUpTypes();
        }

        #endregion


        #region Methods

        /// <summary>
        /// is called by game1, all the functionality is implemented
        /// in this method or a submethod
        /// </summary>
        /// <param name="gameTime">The gametime</param>
        /// <param name="gameObjects">All objects which belong to a level</param>
        /// <param name="activePowerUps">all active powerups</param>
        public void update(GameTime gameTime, LayerList<LayerInterface> gameObjects, List<PowerUp> activePowerUps)
        {
            this.gameObjects = gameObjects;
            this.activePowerUps = activePowerUps;
            if (!paused)
            {
                searchForPickedPowerUps(gameObjects.GetActiveObjects());
                updatePowerUpLifetimes(gameTime.ElapsedGameTime.Milliseconds);
            }
        }

        /// <summary>
        /// Creates a random powerup at the indicated position. The owner will be null. The version (good/bad/neutral) will be random.
        /// </summary>
        /// <param name="position"></param>
        public PowerUp createRandomPowerUp(Vector2 position)
        {
            return createRandomPowerUp(position, getRandomVersion());
        }

        /// <summary>
        /// Creates a random powerup of the specified version at the indicated position. The owner will be null. 
        /// </summary>
        /// <param name="position"></param>
        public PowerUp createRandomPowerUp(Vector2 position, PowerUpVersion version)
        {
            int safetyCounter = 30;
            PowerUpType newPowerUpType;
            do
            {
                newPowerUpType = getRandomPowerUp(version);
                safetyCounter--;
            } while (safetyCounter > 0 && allowedPowerUps.Count != 0 && !allowedPowerUps.Contains(newPowerUpType));

            PowerUp newPowerUp = createPowerUp(position, version, newPowerUpType);

            SuperController.Instance.addGameObjectToGameInstance(newPowerUp);
            return newPowerUp;
        }

        /// <summary>
        /// Creates a new PowerUp with the specified paramaters.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="version"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private PowerUp createPowerUp(Vector2 position, PowerUpVersion version, PowerUpType type)
        {
            TaskManager.Instance.addTask(new SoundTask(0, SoundIndicator.powerUpSpawn, (int)IngameSound.PowerUpSpawn));

            return new PowerUp(getPowerUpName(type),
                true,
                StorageManager.Instance.getTextureByName("PowerupBox_" + type.ToString()),
                null,
                null,
                getSizeFromVersion(version),
                position,
                50,
                Model.MeasurementUnit.Meter,
                getDurationFromType(type),
                type,
                getBehaviourFromType(type),
                version);
        }

        /// <summary>
        /// Returns a random PowerUp of the specified version.
        /// </summary>
        /// <returns></returns>
        private PowerUpType getRandomPowerUp(PowerUpVersion version)
        {
            if (version == PowerUpVersion.Positive)
            {
                return positivePowerUps.ElementAt(pseudoRandom(positivePowerUps.Count));
            }
            else
            {
                return neutralPowerUps.ElementAt(pseudoRandom(neutralPowerUps.Count));
            }
        }

        public String getPowerUpName(PowerUpType type)
        {
            return "Powerup_" + type.ToString();
        }

        /// <summary>
        /// Returns a random PowerUp of the specified version.
        /// </summary>
        /// <returns></returns>
        private Vector2 getSizeFromVersion(PowerUpVersion version)
        {
            if (version == PowerUpVersion.Neutral)
            {
                return (Vector2)SettingsManager.Instance.get("neutralPowerupSize");
            }
            else
            {
                return (Vector2)SettingsManager.Instance.get("positivePowerupSize");
            }
        }

        /// <summary>
        /// Get Behaviour from type. Default is game exclusive.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private PowerUpBehaviour getBehaviourFromType(PowerUpType type)
        {
            switch (type)
            {
                case PowerUpType.BallRain:
                    {
                        return PowerUpBehaviour.NonExclusive;
                    }
                case PowerUpType.DoubleBall:
                    {
                        return PowerUpBehaviour.NonExclusive;
                    }
                case PowerUpType.InvertedControl:
                    {
                        return PowerUpBehaviour.TeamExclusive;
                    }
                case PowerUpType.Jumpheight:
                    {
                        return PowerUpBehaviour.NonExclusive;
                    }
                case PowerUpType.Wind:
                    {
                        return PowerUpBehaviour.GameExclusive;
                    }
                case PowerUpType.SwitchStones:
                    {
                        return PowerUpBehaviour.NonExclusive;
                    }
                case PowerUpType.SunsetSunrise:
                    {
                        return PowerUpBehaviour.GameExclusive;
                    }
                case PowerUpType.Volcano:
                    {
                        return PowerUpBehaviour.NonExclusive;
                    }
                case PowerUpType.SuperBomb:
                    {
                        return PowerUpBehaviour.NonExclusive;
                    }
                default:
                    {
                        return PowerUpBehaviour.GameExclusive;
                    }
            }
        }

        /// <summary>
        /// Returns the duration of a powerup.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int getDurationFromType(PowerUpType type)
        {
            switch (type)
            {
                case PowerUpType.InvertedControl:
                    return (int)SettingsManager.Instance.get("invertedControlEffectDuration");
                case PowerUpType.SwitchStones:
                    return (int)SettingsManager.Instance.get("switchStonesEffectDuration");
                case PowerUpType.Wind:
                    return (int)SettingsManager.Instance.get("windEffectDuration");
                case PowerUpType.SunsetSunrise:
                    return (int)SettingsManager.Instance.get("sunsetSunriseEffectDuration");
                case PowerUpType.BallRain:
                    return (int)SettingsManager.Instance.get("ballRainEffectDuration");
                case PowerUpType.Jumpheight:
                    return (int)SettingsManager.Instance.get("jumpheightEffectDuration");
                case PowerUpType.Volcano:
                    return (int)SettingsManager.Instance.get("volcanoEffectDuration");
                default:
                    return (int)SettingsManager.Instance.get("defaultEffectDuration");
            }
        }


        /// <summary>
        /// Checks if any of the passive PowerUps has been picked up.
        /// </summary>
        /// <param name="activeObjectList">List with all active objects</param>
        private void searchForPickedPowerUps(LayerList<ActiveObject> activeObjectList)
        {
            LayerList<PowerUp> powerUps = gameObjects.GetPowerup();
            for (int x = 0; x < powerUps.Count; x++)
            {
                if (powerUps[x].Owner != null)
                {
                    initializePowerUp(powerUps[x], activeObjectList);
                }
            }
        }

        /// <summary>
        /// Subtracts the passed time from all the PowerUps currently in possesion of a player.
        /// </summary>
        /// <param name="gameTime"></param>
        private void updatePowerUpLifetimes(int passedMilliseconds)
        {
            // Update PowerUps owned by players.
            for (int x = 0; x < activePowerUps.Count; x++)
            {
                activePowerUps[x].LifeTime -= passedMilliseconds;
                if (activePowerUps[x].LifeTime <= 0)
                {
                    disposePowerUp(activePowerUps[x]);
                }
            }


            List<PowerUp> powerUps = gameObjects.GetPowerup();
            // Update PowerUps not owned by players.
            for (int x = 0; x < powerUps.Count; x++)
            {
                powerUps[x].RemoveTime -= passedMilliseconds;
                if (powerUps[x].RemoveTime <= 0)
                {
                    removePowerUpThatWasNotPickedUp(powerUps[x]);
                }
            }
        }

        /// <summary>
        /// This method is called when a PowerUp is picked up by a player. It mostly serves to distribute to subfunctions.
        /// </summary>
        /// <param name="powerUp"></param>
        /// <returns></returns>
        private void initializePowerUp(PowerUp powerUp, LayerList<ActiveObject> activeObjectList)
        {
            // PowerUp is picked up, so its no longer visible and part of the physical world.
            SuperController.Instance.removeGameObjectFromGameInstance(powerUp);

            // Check exclusivity of PowerUp (= behaviour). Some PowerUps may only be active for a certain player/team. Others aren't exclusive.
            switch (powerUp.PowerUpBehaviour)
            {
                case PowerUpBehaviour.GameExclusive:
                    {
                        // Just renew if its the sunset powerup
                        if (powerUp.PowerUpType == PowerUpType.SunsetSunrise)
                        {
                            foreach (PowerUp p in activePowerUps)
                            {
                                if (p.PowerUpType == PowerUpType.SunsetSunrise)
                                {
                                    p.LifeTime = powerUp.LifeTime;
                                    GameLogic.Instance.handlePowerupDisplay(p);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            // No need to check for PowerUp Version, since all instances will be removed.
                            removeAllInstancesOfPowerUp(powerUp.PowerUpType);
                        }


                        break;
                    }
                case PowerUpBehaviour.TeamExclusive:
                    {
                        // Positive -> Remove Powerup from the other team
                        // Neutral -> Remove Powerup from everybody
                        if (powerUp.PowerUpVersion == PowerUpVersion.Positive)
                        {
                            removeAllInstancesOfPowerUpForTeam(powerUp.PowerUpType, getOtherTeam(powerUp.Owner.Team));
                        }
                        else
                        {
                            removeAllInstancesOfPowerUp(powerUp.PowerUpType);
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            // Search for PowerUps of the same type on the player. They get removed, which means they are renewed.
            removeAllInstancesOfPowerUpForPlayer(powerUp.PowerUpType, powerUp.Owner);

            // Add powerUp to active PowerUps
            activePowerUps.Add(powerUp);

            // Tell the graphics that this powerUp was created.
            TaskManager.Instance.addTask(new GraphicsTask(0, GraphicTask.DisplayPowerUp, powerUp, PowerUpState.Starting));            

            // Check type of powerup
            switch (powerUp.PowerUpType)
            {
                case PowerUpType.Wind:
                    {                        
                        setWindPowerUp(powerUp, true);
                        Logger.Instance.log(Sender.PowerupManager, "Started wind PowerUp.", PriorityLevel.Priority_1);
                        break;
                    }
                case PowerUpType.Jumpheight:
                    {
                        setJumpheightPowerUp(powerUp, true);
                        Graphics.Instance.playJumpHighlight(powerUp.Owner.Team);
                        Logger.Instance.log(Sender.PowerupManager, "Started jumpheight PowerUp.", PriorityLevel.Priority_1);
                        break;
                    }
                case PowerUpType.InvertedControl:
                    {
                        TaskManager.Instance.addTask(new SoundTask(0, SoundIndicator.invertedControl, (int)IngameSound.InvertedControl));
                        setInvertedControlPowerUp(powerUp, true);
                        Logger.Instance.log(Sender.PowerupManager, "Started inverted control PowerUp.", PriorityLevel.Priority_1);
                        break;
                    }
                case PowerUpType.DoubleBall:
                    {
                        setDoubleBallPowerUp();
                        Logger.Instance.log(Sender.PowerupManager, "Started DoubleBall PowerUp.", PriorityLevel.Priority_1);
                        break;
                    }
                case PowerUpType.BallRain:
                    {
                        setBallRainPowerUp(powerUp);
                        Logger.Instance.log(Sender.PowerupManager, "Started BallRain PowerUp.", PriorityLevel.Priority_1);
                        break;
                    }
                case PowerUpType.SwitchStones:
                    {                        
                        setSwitchStonesPowerUp(true);
                        Logger.Instance.log(Sender.PowerupManager, "Started SwitchStones PowerUp.", PriorityLevel.Priority_1);
                        break;
                    }
                case PowerUpType.SunsetSunrise:
                    {
                        setSunsetSunrisePowerUp(true);
                        Logger.Instance.log(Sender.PowerupManager, "Started SunsetSunrise PowerUp.", PriorityLevel.Priority_1);
                        break;
                    }
                case PowerUpType.Volcano:
                    {
                        setVolcanoPowerUp();
                        Logger.Instance.log(Sender.PowerupManager, "Started Volcano PowerUp.", PriorityLevel.Priority_1);
                        break;
                    }
                case PowerUpType.SuperBomb:
                    {
                        setSuperBombPowerUp();
                        Logger.Instance.log(Sender.PowerupManager, "Started SuperBomb PowerUp.", PriorityLevel.Priority_1);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            // Visualize this powerup
            GameLogic.Instance.handlePowerupDisplay(powerUp);
        }


        /// <summary>
        /// Reverts all the changes caused by a specific PowerUp. It mostly serves to distribute to subfunctions.
        /// </summary>
        /// <param name="powerUp"></param>
        private void disposePowerUp(PowerUp powerUp)
        {
            activePowerUps.Remove(powerUp);

            // Send a task to the graphics to visualize the disposal:
            TaskManager.Instance.addTask(new GraphicsTask(0, GraphicTask.DisplayPowerUp, powerUp, PowerUpState.Finshing));

            switch (powerUp.PowerUpType)
            {
                case PowerUpType.Wind:
                    {
                        setWindPowerUp(powerUp, false);
                        Logger.Instance.log(Sender.PowerupManager, "Disposed of wind PowerUp.", PriorityLevel.Priority_1);
                        break;
                    }
                case PowerUpType.Jumpheight:
                    {
                        setJumpheightPowerUp(powerUp, false);
                        ParticleSystemsManager.Instance.stopJumpHighlight(powerUp.Owner.Team);
                        Logger.Instance.log(Sender.PowerupManager, "Disposed of jumpheight PowerUp.", PriorityLevel.Priority_1);
                        break;
                    }
                case PowerUpType.InvertedControl:
                    {
                        ParticleSystemsManager.Instance.stopParalysis(powerUp.Owner.Team);
                        setInvertedControlPowerUp(powerUp, false);
                        Logger.Instance.log(Sender.PowerupManager, "Disposed of inverted control PowerUp.", PriorityLevel.Priority_1);
                        break;
                    }
                case PowerUpType.SwitchStones:
                    {
                        setSwitchStonesPowerUp(false);
                        Logger.Instance.log(Sender.PowerupManager, "Disposed of switch stones PowerUp.", PriorityLevel.Priority_1);
                        break;
                    }
                case PowerUpType.SunsetSunrise:
                    {
                        setSunsetSunrisePowerUp(false);
                        Logger.Instance.log(Sender.PowerupManager, "Disposed of sunset sunrise PowerUp.", PriorityLevel.Priority_1);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// Removes a powerup that has not been picked up.
        /// </summary>
        /// <param name="powerup"></param>
        private void removePowerUpThatWasNotPickedUp(PowerUp powerup)
        {
            SuperController.Instance.removeGameObjectFromGameInstance(powerup);
            TaskManager.Instance.addTask(new GraphicsTask(0, GraphicTask.DisplayPowerUp, powerup, PowerUpState.NotPickedUp));
        }

        /// <summary>
        /// Resets the PowerUpManager. All passive and active powerUps and effects are removed.
        /// </summary>
        public void clear()
        {

        }

        /// <summary>
        /// Initializes the Controller to handle a new GameInstance
        /// </summary>
        public void initialize()
        {

        }

        /// <summary>
        /// Set all Powerups that are allowed in this level. If the list is empty, all powerups are allowed.
        /// ATTENTION: Make sure to add AT LEAST on powerUp that has positive/negative/neutral version. Else
        /// the PowerUpManager might have to create a different Powerup, to avoid an endless loop.
        /// </summary>
        /// <param name="types">List of the allowed PowerUps.</param>
        public void setAllowedPowerUps(List<PowerUpType> types)
        {
            this.allowedPowerUps = types;
        }

        /// <summary>
        /// Pause this controller.
        /// </summary>
        /// <param name="on"> true := controller is paused.</param>
        public void pause(bool on)
        {
            this.paused = on;
        }

        #endregion

        /// <summary>
        /// Registering of powerUpTypes.
        /// </summary>
        private void setPowerUpTypes()
        {
            positivePowerUps = new List<PowerUpType>();
            neutralPowerUps = new List<PowerUpType>();

            positivePowerUps.Add(PowerUpType.Jumpheight);
            positivePowerUps.Add(PowerUpType.InvertedControl);
            positivePowerUps.Add(PowerUpType.Wind);

            neutralPowerUps.Add(PowerUpType.BallRain);
            neutralPowerUps.Add(PowerUpType.DoubleBall);
            neutralPowerUps.Add(PowerUpType.SuperBomb);
            neutralPowerUps.Add(PowerUpType.SwitchStones);
            neutralPowerUps.Add(PowerUpType.Volcano);
            neutralPowerUps.Add(PowerUpType.SunsetSunrise);
        }

        #region PowerUps

        /// <summary>
        /// Sets the Wind powerup.
        /// </summary>
        /// <param name="powerUp"></param>
        /// <param name="on"></param>
        private void setWindPowerUp(PowerUp powerUp, bool on)
        {
            Physics.Instance.setWindPowerUpForTeam(powerUp.Owner.Team, on);

            if (on)
            {
                // Adjust name
                if (powerUp.Owner.Team == 1)
                {
                    powerUp.Name = "Powerup_WindRight";
                }
                else
                {
                    powerUp.Name = "Powerup_WindLeft";
                }

                ParticleSystemsManager.Instance.applyForceFallingParticles((powerUp.Owner.Team == 1) ? new Vector2(0.4f, 0f) : new Vector2(-0.4f, 0));
                if (SuperController.Instance.GameInstance.LevelName == "Beach") ParticleSystemsManager.Instance.activateSandStorm((powerUp.Owner.Team == 1) ? 1 : -1);

            }
            else
            {
                ParticleSystemsManager.Instance.removeForceFallingParticles();
                if (SuperController.Instance.GameInstance.LevelName == "Beach") ParticleSystemsManager.Instance.deactivateSandStorm();
            }
        }

        /// <summary>
        /// Sets the DoubleBall powerup.
        /// </summary>
        /// <param name="powerUp"></param>
        /// <param name="on"></param>
        private void setDoubleBallPowerUp()
        {
            Physics.Instance.setDoubleBallPowerUp();
        }

        /// <summary>
        /// Sets the BallRain powerup.
        /// </summary>
        /// <param name="powerUp"></param>
        /// <param name="on"></param>
        private void setBallRainPowerUp(PowerUp powerUp)
        {
            // First Ball
            TaskManager.Instance.addTask(new PhysicsTask(1000, PhysicsTask.PhysicTaskType.CreateNewBall, Physics.Instance.getRandomPositionAtTheTop()));

            // Consecutive balls
            for (int x = (int)settingsManager.get("ballRainAmount") - 1; x > 0; x--)
            {
                TaskManager.Instance.addTask(new PhysicsTask(x * (int)settingsManager.get("ballRainTime"), PhysicsTask.PhysicTaskType.CreateNewBall, Physics.Instance.getRandomPositionAtTheTop()));
            }
        }

        /// <summary>
        /// Sets the BallRain powerup.
        /// </summary>
        /// <param name="powerUp"></param>
        /// <param name="on"></param>
        private void setSuperBombPowerUp()
        {
            TaskManager.Instance.addTask(new PhysicsTask(1000, PhysicsTask.PhysicTaskType.CreateSuperBomb));
        }

        /// <summary>
        /// Sets the jumpheight powerup.
        /// </summary>
        /// <param name="powerUp"></param>
        /// <param name="on"></param>
        private void setJumpheightPowerUp(PowerUp powerUp, bool on)
        {
            Physics.Instance.setJumpheightPowerUpForPlayer(powerUp.Owner, on);
        }

        /// <summary>
        /// Sets the inverted control powerup.
        /// </summary>
        /// <param name="powerUp"></param>
        /// <param name="on"></param>
        private void setInvertedControlPowerUp(PowerUp powerUp, bool on)
        {
            if (on)
            {
                //start paralysis orbitter
                ParticleSystemsManager.Instance.playParalysis(powerUp.Owner.Team);
                // Activate powerup
                foreach (Player player in getPlayersInTeam(getOtherTeam(powerUp.Owner.Team)))
                {
                    player.Flags.Add(PlayerFlag.InvertedControl);
                }
            }
            else
            {
                ParticleSystemsManager.Instance.stopParalysis(powerUp.Owner.Team);
                // Deactivate powerup
                foreach (Player player in getPlayersInTeam(getOtherTeam(powerUp.Owner.Team)))
                {
                    player.Flags.Remove(PlayerFlag.InvertedControl);
                }
            }

        }

        /// <summary>
        /// Sets the SwitchStones powerup.
        /// </summary>
        /// <param name="powerUp"></param>
        /// <param name="on"></param>
        private void setSwitchStonesPowerUp(bool on)
        {
            GameLogic.Instance.changeStonePositions(on);
        }

        /// <summary>
        /// Sets the SunetSunrise powerup.
        /// </summary>
        /// <param name="powerUp"></param>
        /// <param name="on"></param>
        private void setSunsetSunrisePowerUp(bool on)
        {
            if (on)
            {
                TaskManager.Instance.addTask(new SoundTask(0, SoundIndicator.sunsetPowerup, (int)IngameSound.SunsetPowerUp, getDurationFromType(PowerUpType.SunsetSunrise)));
                TaskManager.Instance.addTask(new GraphicsTask(0, GraphicTask.Sunset, 0));
            }
            else
            {
                TaskManager.Instance.addTask(new GraphicsTask(0, GraphicTask.Sunrise, 0));
            } 
        }

        /// <summary>
        /// Sets the Bombs powerup.
        /// </summary>
        /// <param name="powerUp"></param>
        /// <param name="on"></param>
        private void setVolcanoPowerUp()
        {
            for (int x = (int)settingsManager.get("lavaAmount") - 1; x >= 0; x--)
            {
                TaskManager.Instance.addTask(new PhysicsTask(x * (int)settingsManager.get("lavaTime"), PhysicsTask.PhysicTaskType.CreateLava));
            }
        }

        #endregion

        /// No Changes to specific PowerUps here! ///
        #region Helper Methods

        // Small helper methods


        /// <summary>
        /// Removes all instances of a PowerUpType for a team from the game.
        /// </summary>
        /// <param name="powerUp"></param>
        private void removeAllInstancesOfPowerUpForTeam(PowerUpType powerUpType, int team)
        {
            for (int x = 0; x < activePowerUps.Count; x++)
            {
                if (activePowerUps[x].PowerUpType == powerUpType && activePowerUps[x].Owner.Team == team)
                {
                    disposePowerUp(activePowerUps[x]);
                }
            }
        }


        /// <summary>
        /// Removes all instances of a PowerUpType for all players, minus the one that is creatinga new one, from the game.
        /// </summary>
        /// <param name="powerUp"></param>
        private void removeAllInstancesOfPowerUpForAllButOnePlayer(PowerUpType powerUpType, Player dontRemoveForThisPlayer)
        {
            for (int x = 0; x < activePowerUps.Count; x++)
            {
                if (activePowerUps[x].PowerUpType == powerUpType && activePowerUps[x].Owner != dontRemoveForThisPlayer)
                {
                    disposePowerUp(activePowerUps[x]);
                }
            }
        }

        /// <summary>
        /// Removes all instances of a PowerUpType from one player.
        /// </summary>
        /// <param name="powerUp"></param>
        private void removeAllInstancesOfPowerUpForPlayer(PowerUpType powerUpType, Player player)
        {
            for (int x = 0; x < activePowerUps.Count; x++)
            {
                if (activePowerUps[x].PowerUpType == powerUpType && activePowerUps[x].Owner == player)
                {
                    disposePowerUp(activePowerUps[x]);
                }
            }
        }


        /// <summary>
        /// Removes all instances of a PowerUpType from the game.
        /// </summary>
        /// <param name="powerUp"></param>
        private void removeAllInstancesOfPowerUp(PowerUpType powerUpType)
        {
            for (int x = 0; x < activePowerUps.Count; x++)
            {
                if (activePowerUps[x].PowerUpType == powerUpType)
                {
                    disposePowerUp(activePowerUps[x]);
                }
            }
        }

        /// <summary>
        /// Get a random version of a PowerUp.
        /// </summary>
        /// <returns></returns>
        private PowerUpVersion getRandomVersion()
        {
            switch (pseudoRandom(2))
            {
                case 0:
                    {
                        return PowerUpVersion.Positive;
                    }
                default:
                    {
                        return PowerUpVersion.Neutral;
                    }
            }
        }

        /// <summary>
        /// returns the other team.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        private int getOtherTeam(int team)
        {
            return (team % 2) + 1;
        }

        /// <summary>
        /// Get a random number
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        private int pseudoRandom(int maxValue)
        {
            if (maxValue == 0)
            {
                throw new Exception("No positive or neutral powerups specified!");
            }
            return randomizer.Next(100) % maxValue;
        }

        /// <summary>
        /// Returns all players that are in the same team as the parameter.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private List<Player> getPlayersInTeam(int team)
        {
            List<Player> players = new List<Player>();

            // Get teams.
            foreach (Player currentPlayer in gameObjects.GetPlayer())
            {
                if (currentPlayer.Team == team)
                {
                    players.Add(currentPlayer);
                }
            }
            return players;
        }

        #endregion

    }
}
