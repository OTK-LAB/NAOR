using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateCC
{
    [System.Serializable]
    public class PlayerData
    {
        #region Variable Organization Classes
        //This region includes all editable and non-editable variables organized into separate classes.
        //Animation curves used to make movement more controllable and smooth
        [System.Serializable]
        public class PhysicsVariables
        {
            [System.Serializable]
            public class SlopeVariables
            {
                [SerializeField, Range(0, 90)] private float maxSlopeAngle;
                [SerializeField, NonEditable] private float currentSlopeAngle;
                [SerializeField, Range(0, 180)] private float maxEasedSlopeAngleChange;
                [SerializeField, NonEditable] private float currentSlopeAngleChange;
                [SerializeField, Range(0, 1)] private float rotationMultiplierOnSlope;
                [SerializeField] private float rotationSpeed;
                [SerializeField, NonEditable] private bool stayStill;

                public float MaxSlopeAngle => maxSlopeAngle;
                public float CurrentSlopeAngle { get { return currentSlopeAngle; } set { currentSlopeAngle = value; } }
                public float MaxEasedSlopeAngleChange => maxEasedSlopeAngleChange;
                public float CurrentSlopeAngleChange { get { return currentSlopeAngleChange; } set { currentSlopeAngleChange = value; } }
                public float RotationMultiplierOnSlope { get { return rotationMultiplierOnSlope; } set { rotationMultiplierOnSlope = value; } }
                public float RotationSpeed { get { return rotationSpeed; } set { rotationSpeed = value; } }
                public bool StayStill { get { return stayStill; } set { stayStill = value; } }
            }
            [System.Serializable]
            public class PlatformVariables
            {
                [SerializeField, NonEditable] private Vector2 maxPlatformVelocity;
                [SerializeField, NonEditable] private Vector2 dampedVelocity;
                [SerializeField, BoundedCurve, Space(5)] private AnimationCurve dampingCurve;
                [SerializeField] private float dampingTime;

                public Vector2 MaxPlatformVelocity { get { return maxPlatformVelocity; } set { maxPlatformVelocity = value; } }
                public Vector2 DampedVelocity { get { return dampedVelocity; } set { dampedVelocity = value; } }
                public AnimationCurve DampingCurve => dampingCurve;
                public float DampingTime => dampingTime;
            }

            [SerializeField] private LayerMask groundLayerMask;
            [SerializeField] private LayerMask headBumpLayerMask;
            [SerializeField] private LayerMask wallLayerMask;
            [SerializeField] private LayerMask hingeLayerMask;
            [SerializeField] private LayerMask enemyLayerMask;
            [SerializeField, NonEditable] private Vector3 groundCheckPosition;
            [SerializeField, NonEditable] private Vector3 headCheckPosition;
            [SerializeField, NonEditable] private Vector2 walkSpeedDirection;
            [SerializeField, NonEditable] private Vector2 contactPosition;
            [SerializeField, NonEditable] private List<ContactPoint2D> contacts = new();
            [SerializeField, NonEditable] private int facingDirection;
            [SerializeField, Range(0, 90)] private float headBumpMinAngle;
            [SerializeField, Range(0, 90)] private float slideOnCornerMinAngle;
            [SerializeField] private float slideSpeedOnCorner;
            [SerializeField, NonEditable] private bool isGrounded, isOnNotWalkableSlope, isMultipleContactWithNonWalkableSlope, isMultipleContactWithWalkableSlope, isOnHeadBump, isOnCorner, isNextToWall;
            [SerializeField, NonEditable] private int wallDirection;
            [SerializeField, NonEditable] private bool canJump, cutJump, canBumpHead, canSlideCorner, canWallJump;
            [SerializeField, NonEditable] private Rigidbody2D collidedMovingRigidbody;
            [SerializeField, NonEditable] private Vector2 localVelocity;
            [SerializeField, Inform("Experimental")] private bool useCustomZRotations;
            public SlopeVariables Slope;
            public PlatformVariables Platform;
            [SerializeField] private LayerMask ledgeLayer;
            [SerializeField] private float ledgeDetectionDistance;
            [SerializeField, NonEditable] private Vector2 ledgeHangPosition;
            [SerializeField, NonEditable] private bool canPlungeAttackByHeight;
            [SerializeField, NonEditable] private bool canGlideByHeight;
            [SerializeField, NonEditable] private HingeJoint2D connectedHingeJoint;
            [HideInInspector]public RaycastHit2D Ledge;
            [SerializeField, NonEditable] private GameObject connectedGroundObject;


            public LayerMask GroundLayerMask { get { return groundLayerMask; } set { groundLayerMask = value; } }
            public LayerMask HeadBumpLayerMask => headBumpLayerMask;
            public LayerMask WallLayerMask => wallLayerMask;
            public LayerMask HingeLayerMask => hingeLayerMask;
            public LayerMask EnemyLayerMask => enemyLayerMask;
            public Vector2 GroundCheckPosition { get { return groundCheckPosition; } set { groundCheckPosition = value; } }
            public Vector2 HeadCheckPosition { get { return headCheckPosition; } set { headCheckPosition = value; } }
            public Vector2 WalkSpeedDirection { get { return walkSpeedDirection; } set { walkSpeedDirection = value; } }
            public Vector2 ContactPosition { get { return contactPosition; } set { contactPosition = value; } }
            public List<ContactPoint2D> Contacts { get { return contacts; } set { contacts = value; } }
            public int FacingDirection { get { return facingDirection; } set { facingDirection = value; } }
            public float HeadBumpMinAngle => headBumpMinAngle;
            public float SlideOnCornerMinAngle => slideOnCornerMinAngle;
            public float SlideSpeedOnCorner => slideSpeedOnCorner;
            public bool IsGrounded { get { return isGrounded; } set { isGrounded = value; } }
            public bool IsOnNotWalkableSlope { get { return isOnNotWalkableSlope; } set { isOnNotWalkableSlope = value; } }
            public bool IsMultipleContactWithNonWalkableSlope { get { return isMultipleContactWithNonWalkableSlope; } set { isMultipleContactWithNonWalkableSlope = value; } }
            public bool IsMultipleContactWithWalkableSlope { get { return isMultipleContactWithWalkableSlope; } set { isMultipleContactWithWalkableSlope = value; } }
            public bool IsOnHeadBump { get { return isOnHeadBump; } set { isOnHeadBump = value; } }
            public bool IsOnCorner { get { return isOnCorner; } set { isOnCorner = value; } }
            public bool IsNextToWall { get { return isNextToWall; } set { isNextToWall = value; } }
            public int WallDirection { get { return wallDirection; } set { wallDirection = value; } }
            public bool CanJump { get { return canJump; } set { canJump = value; } }
            public bool CutJump { get { return cutJump; } set { cutJump = value; } }
            public bool CanBumpHead { get { return canBumpHead; } set { canBumpHead = value; } }
            public bool CanSlideCorner { get { return canSlideCorner; } set { canSlideCorner = value; } }
            public bool CanWallJump { get { return canWallJump; } set { canWallJump = value; } }
            public Rigidbody2D CollidedMovingRigidbody { get { return collidedMovingRigidbody; } set { collidedMovingRigidbody = value; } }
            public Vector2 LocalVelocity { get { return localVelocity; } set { localVelocity = value; } }
            public bool UseCustomZRotations => useCustomZRotations;
            public LayerMask LedgeLayer => ledgeLayer;
            public float LedgeDetectionDistance { get { return ledgeDetectionDistance; } }
            public Vector2 LedgeHangPosition { get { return ledgeHangPosition; } set { ledgeHangPosition = value; } }
            public bool CanPlungeAttack { get { return canPlungeAttackByHeight; } set { canPlungeAttackByHeight = value; } }
            public bool CanGlideByHeight { get { return canGlideByHeight; } set { canGlideByHeight = value; } }
            public HingeJoint2D ConnectedHingeJoint {  get { return connectedHingeJoint; } set { connectedHingeJoint = value; } }
            public GameObject ConnectedGroundObject { get { return connectedGroundObject; } set { connectedGroundObject = value; } }
        }
        [System.Serializable]
        public class WalkVariables
        {
            [SerializeField] private float maxSpeed;
            [SerializeField, BoundedCurve, Space(5)] private AnimationCurve speedUpCurve;
            [SerializeField] private float speedUpTime;
            [SerializeField, BoundedCurve, Space(5)] private AnimationCurve slowDownCurve;
            [SerializeField] private float slowDownTime;
            [SerializeField, BoundedCurve(0, -1, 2, 2), Space(5)] private AnimationCurve turnBackCurve;
            [SerializeField] private float turnBackTime;
            [SerializeField, NonEditable] private float physics2DGravityScale;

            public float MaxSpeed => maxSpeed;
            public AnimationCurve SpeedUpCurve => speedUpCurve;
            public float SpeedUpTime => speedUpTime;
            public AnimationCurve SlowDownCurve => slowDownCurve;
            public float SlowDownTime => slowDownTime;
            public AnimationCurve TurnBackCurve => turnBackCurve;
            public float TurnBackTime => turnBackTime;
            public float Physics2DGravityScale => physics2DGravityScale;
        }
        [System.Serializable]
        public class CrouchVariables
        {
            [SerializeField] private float maxSpeed;
            [SerializeField, BoundedCurve, Space(5)] private AnimationCurve speedUpCurve;
            [SerializeField] private float speedUpTime;
            [SerializeField, BoundedCurve, Space(5)] private AnimationCurve slowDownCurve;
            [SerializeField] private float slowDownTime;
            [SerializeField, BoundedCurve(0, -1, 2, 2), Space(5)] private AnimationCurve turnBackCurve;
            [SerializeField] private float turnBackTime;
            [SerializeField, NonEditable] private float physics2DGravityScale;

            public float MaxSpeed => maxSpeed;
            public AnimationCurve SpeedUpCurve => speedUpCurve;
            public float SpeedUpTime => speedUpTime;
            public AnimationCurve SlowDownCurve => slowDownCurve;
            public float SlowDownTime => slowDownTime;
            public AnimationCurve TurnBackCurve => turnBackCurve;
            public float TurnBackTime => turnBackTime;
            public float Physics2DGravityScale => physics2DGravityScale;
        }
        [System.Serializable]
        public class JumpVariables
        {
            [System.Serializable]
            public class JumpInfo
            {
                public PlayerMain.AnimName Animation;
                [SerializeField] private float maxHeight;
                [SerializeField, BoundedCurve] private AnimationCurve jumpHeightCurve;
                [SerializeField] private float jumpTime;
                [SerializeField, NonEditable] private AnimationCurve jumpVelocityCurve;
                [SerializeField, Range(0, 1)] private float jumpCutPower;
                [SerializeField, Range(0, 1)] private float timeScaleOnCut;

                public float MaxHeight => maxHeight;
                public AnimationCurve JumpHeightCurve => jumpHeightCurve;
                public float JumpTime => jumpTime;
                public AnimationCurve JumpVelocityCurve { get { return jumpVelocityCurve; } set { jumpVelocityCurve = value; } }
                public float JumpCutPower => jumpCutPower;
                public float TimeScaleOnCut => timeScaleOnCut;
            }
            [SerializeField, NonEditable] private int nextJumpInt;
            public List<JumpInfo> Jumps;
            [SerializeField, NonEditable] private bool newJump;
            [SerializeField, Range(0, 0.5f)] private float coyoteTimeMaxTime, jumpBufferMaxTime;
            [SerializeField, NonEditable] private float coyoteTimeTimer, jumpBufferTimer;
            [SerializeField] private float maxXSpeed;
            [SerializeField, BoundedCurve, Space(5)] private AnimationCurve xSpeedUpCurve;
            [SerializeField] private float xSpeedUpTime;
            [SerializeField, BoundedCurve, Space(5)] private AnimationCurve xSlowDownCurve;
            [SerializeField] private float xSlowDownTime;
            [SerializeField, BoundedCurve(0, -1, 2, 2), Space(5)] private AnimationCurve xTurnBackCurve;
            [SerializeField] private float xTurnBackTime;
            [SerializeField] private float ignoreWallSlideTime;
            [SerializeField, NonEditable] private float physics2DGravityScale;

            public int NextJumpInt { get { return nextJumpInt; } set { nextJumpInt = value; } }
            public bool NewJump { get { return newJump; } set { newJump = value; } }
            public float CoyoteTimeMaxTime => coyoteTimeMaxTime;
            public float JumpBufferMaxTime => jumpBufferMaxTime;
            public float CoyoteTimeTimer { get { return coyoteTimeTimer; } set { coyoteTimeTimer = value; } }
            public float JumpBufferTimer { get { return jumpBufferTimer; } set { jumpBufferTimer = value; } }
            public float MaxXSpeed => maxXSpeed;
            public AnimationCurve XSpeedUpCurve => xSpeedUpCurve;
            public float XSpeedUpTime => xSpeedUpTime;
            public AnimationCurve XSlowDownCurve => xSlowDownCurve;
            public float XSlowDownTime => xSlowDownTime;
            public AnimationCurve XTurnBackCurve => xTurnBackCurve;
            public float XTurnBackTime => xTurnBackTime;
            public float IgnoreWallSlideTime => ignoreWallSlideTime;
            public float Physics2DGravityScale => physics2DGravityScale;

        }
        [System.Serializable]
        public class LandVariables
        {
            [SerializeField, BoundedCurve] private AnimationCurve landHeightCurve;
            [SerializeField] private float landTime;
            [SerializeField, NonEditable] private AnimationCurve landVelocityCurve;
            [SerializeField] private float minLandSpeed;
            [SerializeField] private float maxXSpeed;
            [SerializeField, BoundedCurve, Space(5)] private AnimationCurve xSpeedUpCurve;
            [SerializeField] private float xSpeedUpTime;
            [SerializeField, BoundedCurve, Space(5)] private AnimationCurve xSlowDownCurve;
            [SerializeField] private float xSlowDownTime;
            [SerializeField, BoundedCurve(0, -1, 2, 2), Space(5)] private AnimationCurve xTurnBackCurve;
            [SerializeField] private float xTurnBackTime;
            [SerializeField, NonEditable] private float physics2DGravityScale;

            public AnimationCurve LandHeightCurve => landHeightCurve;
            public float LandTime => landTime;
            public AnimationCurve LandVelocityCurve { get { return landVelocityCurve; } set { landVelocityCurve = value; } }
            public float MinLandSpeed => minLandSpeed;
            public float MaxXSpeed => maxXSpeed;
            public AnimationCurve XSpeedUpCurve => xSpeedUpCurve;
            public float XSpeedUpTime => xSpeedUpTime;
            public AnimationCurve XSlowDownCurve => xSlowDownCurve;
            public float XSlowDownTime => xSlowDownTime;
            public AnimationCurve XTurnBackCurve => xTurnBackCurve;
            public float XTurnBackTime => xTurnBackTime;
            public float Physics2DGravityScale => physics2DGravityScale;
        }
        [System.Serializable]
        public class DashVariables
        {
            [SerializeField, Space(5)] private float maxHeight;
            [SerializeField, BoundedCurve(0, -1, 1, 2)] private AnimationCurve dashHeightCurve;
            [SerializeField, NonEditable] private AnimationCurve dashYVelovityCurve;
            [SerializeField, Space(5)] private float maxSpeed;
            [SerializeField, BoundedCurve] private AnimationCurve dashXVelocityCurve;
            [SerializeField, Space(5)] private float dashTime;
            [SerializeField] private float dashCooldown;
            [SerializeField, NonEditable] private float dashCooldownTimer;
            [SerializeField, NonEditable] private float physics2DGravityScale;

            public float MaxHeight => maxHeight;
            public AnimationCurve DashHeightCurve => dashHeightCurve;
            public AnimationCurve DashYVelocityCurve { get { return dashYVelovityCurve; } set { dashYVelovityCurve = value; } }
            public float MaxSpeed => maxSpeed;
            public AnimationCurve DashXVelocityCurve => dashXVelocityCurve;
            public float DashTime => dashTime;
            public float DashCooldown => dashCooldown;
            public float DashCooldownTimer { get { return dashCooldownTimer; } set { dashCooldownTimer = value; } }
            public float Physics2DGravityScale => physics2DGravityScale;
        }
        [System.Serializable]
        public class SwingVariables
        {
            [SerializeField] private float drag;
            [SerializeField] private float gravity;

            private Vector2 swingInitialPosition;
            public Vector2 SwingInitialPosition { get { return swingInitialPosition; } set { swingInitialPosition = value; } }
            public float Drag => drag;
            public float Gravity => gravity;
        }
        [System.Serializable]
        public class GlideVariables
        {
            [SerializeField] private float minHeight;
            [SerializeField] private float manaDrainPerSecond;
            [SerializeField] private float fallSpeedMultiplier;
            [SerializeField] private float glideBufferTime;
            [SerializeField] private float glideBufferMaxTime;

            public float MinHeight => minHeight;
            public float ManaDrainPerSecond => manaDrainPerSecond;
            public float FallSpeedMultiplier => fallSpeedMultiplier;
            public float GlideBufferTimer { get { return glideBufferTime; } set { glideBufferTime = value; } }
            public float GlideBufferMaxTime => glideBufferMaxTime;
        }
        [System.Serializable]
        public class WallMovementVariables
        {
            [System.Serializable]
            public class WallSlideVariables
            {
                [SerializeField, CustomRange(-1, 1)] private int facingDirectionWhenSliding;
                [SerializeField] private float maxSpeed;
                [SerializeField, BoundedCurve, Space(5)] private AnimationCurve speedUpCurve;
                [SerializeField] private float speedUpTime;
                [SerializeField, BoundedCurve, Space(5)] private AnimationCurve slowDownCurve;
                [SerializeField] private float slowDownTime;
                [SerializeField, BoundedCurve(0, -1, 2, 2), Space(5)] private AnimationCurve turnBackCurve;
                [SerializeField] private float turnBackTime;

                public int FacingDirectionWhenSliding => facingDirectionWhenSliding;
                public float MaxSpeed => maxSpeed;
                public AnimationCurve SpeedUpCurve => speedUpCurve;
                public float SpeedUpTime => speedUpTime;
                public AnimationCurve SlowDownCurve => slowDownCurve;
                public float SlowDownTime => slowDownTime;
                public AnimationCurve TurnBackCurve => turnBackCurve;
                public float TurnBackTime => turnBackTime;
            }
            [System.Serializable]
            public class WallGrabVariables
            {
                [SerializeField, BoundedCurve, Space(5)] private AnimationCurve slowDownCurve;
                [SerializeField] private float slowDownTime;

                public AnimationCurve SlowDownCurve => slowDownCurve;
                public float SlowDownTime => slowDownTime;
            }
            [System.Serializable]
            public class WallClimbVariables
            {
                [SerializeField] private float maxSpeed;
                [SerializeField, BoundedCurve, Space(5)] private AnimationCurve speedUpCurve;
                [SerializeField] private float speedUpTime;
                [SerializeField, BoundedCurve, Space(5)] private AnimationCurve slowDownCurve;
                [SerializeField] private float slowDownTime;
                [SerializeField, BoundedCurve(0, -1, 2, 2), Space(5)] private AnimationCurve turnBackCurve;
                [SerializeField] private float turnBackTime;

                public float MaxSpeed => maxSpeed;
                public AnimationCurve SpeedUpCurve => speedUpCurve;
                public float SpeedUpTime => speedUpTime;
                public AnimationCurve SlowDownCurve => slowDownCurve;
                public float SlowDownTime => slowDownTime;
                public AnimationCurve TurnBackCurve => turnBackCurve;
                public float TurnBackTime => turnBackTime;
            }
            [System.Serializable]
            public class WallJumpVariables
            {
                [SerializeField] private float maxHeight;
                [SerializeField, BoundedCurve] private AnimationCurve jumpHeightCurve;
                [SerializeField] private float jumpTime;
                [SerializeField, NonEditable] private AnimationCurve jumpVelocityCurve;
                [SerializeField, Range(0, 1)] private float jumpCutPower;
                [SerializeField, Range(0, 1)] private float timeScaleOnCut;
                [SerializeField, Range(0, 0.5f)] private float coyoteTimeMaxTime, jumpBufferMaxTime;
                [SerializeField, NonEditable] private float coyoteTimeTimer, jumpBufferTimer;
                [SerializeField] private float xStartVelocity;
                [SerializeField, BoundedCurve] private AnimationCurve xStartVelocityDampingCurve;
                [SerializeField] private float dampingTime;
                [SerializeField, Range(0, 1)] private float oppositeSpeedMultiplierWhenDamping;
                [SerializeField] private float xMaxSpeed;
                [SerializeField, BoundedCurve, Space(5)] private AnimationCurve xSpeedUpCurve;
                [SerializeField] private float xSpeedUpTime;
                [SerializeField, BoundedCurve, Space(5)] private AnimationCurve xSlowDownCurve;
                [SerializeField] private float xSlowDownTime;
                [SerializeField, BoundedCurve(0, -1, 2, 2), Space(5)] private AnimationCurve xTurnBackCurve;
                [SerializeField] private float xTurnBackTime;

                public float MaxHeight => maxHeight;
                public AnimationCurve JumpHeightCurve => jumpHeightCurve;
                public float JumpTime => jumpTime;
                public AnimationCurve JumpVelocityCurve { get { return jumpVelocityCurve; } set { jumpVelocityCurve = value; } }
                public float JumpCutPower => jumpCutPower;
                public float TimeScaleOnCut => timeScaleOnCut;
                public float CoyoteTimeMaxTime => coyoteTimeMaxTime;
                public float JumpBufferMaxTime => jumpBufferMaxTime;
                public float CoyoteTimeTimer { get { return coyoteTimeTimer; } set { coyoteTimeTimer = value; } }
                public float JumpBufferTimer { get { return jumpBufferTimer; } set { jumpBufferTimer = value; } }
                public float XStartVelocity => xStartVelocity;
                public AnimationCurve XStartVelocityDampingCurve => xStartVelocityDampingCurve;
                public float DampingTime => dampingTime;
                public float OppositeSpeedMultiplierWhenDamping => oppositeSpeedMultiplierWhenDamping;
                public float XMaxSpeed => xMaxSpeed;
                public AnimationCurve XSpeedUpCurve => xSpeedUpCurve;
                public float XSpeedUpTime => xSpeedUpTime;
                public AnimationCurve XSlowDownCurve => xSlowDownCurve;
                public float XSlowDownTime => xSlowDownTime;
                public AnimationCurve XTurnBackCurve => xTurnBackCurve;
                public float XTurnBackTime => xTurnBackTime;
            }
            public enum ExhaustTriggerType { None, TimeBased, ClimbAmountBased }

            public WallSlideVariables WallSlide;
            public WallGrabVariables WallGrab;
            public WallClimbVariables WallClimb;
            public WallJumpVariables WallJump;
            [SerializeField] private float maxStamina;
            [SerializeField, NonEditable] private float currentStamina;
            [SerializeField] private ExhaustTriggerType exhaustTrigger;
            [SerializeField, Inform("stamina/sec or stamina/unit")] private float staminaDrainPerTrigger;
            [SerializeField] private float staminaRegenPerSec;
            [SerializeField] private bool allowJumpWhenExhausted;
            [SerializeField, NonEditable] private float physics2DGravityScale;

            public float MaxStamina => maxStamina;
            public float CurrentStamina { get { return currentStamina; } set { currentStamina = value; } }
            public ExhaustTriggerType ExhaustTrigger => exhaustTrigger;
            public float StaminaDrainPerTrigger => staminaDrainPerTrigger;
            public float StaminaRegenPerSec => staminaRegenPerSec;
            public bool AllowJumpWhenExhausted => allowJumpWhenExhausted;
            public float Physics2DGravityScale => physics2DGravityScale;
        }
        #region Attack Variables
        [System.Serializable]
        public class AttackStateVariables
        {
            [System.Serializable]
            public class BasicAttack1Variables
            {
                [SerializeField] private float attackDuration;
                [SerializeField] private float maxStateTime;

                public float AttackDuration => attackDuration;
                public float MaxStateTime => maxStateTime;
            }
            [System.Serializable]
            public class BasicAttack2Variables
            {
                [SerializeField] private float attackDuration;
                [SerializeField] private float maxStateTime;

                public float AttackDuration => attackDuration;
                public float MaxStateTime => maxStateTime;
            }
            [System.Serializable]
            public class BasicAttack3Variables
            {
                [SerializeField] private float attackDuration;
                [SerializeField] private float maxStateTime;

                public float AttackDuration => attackDuration;
                public float MaxStateTime => maxStateTime;
            }
            [System.Serializable]
            public class ChargeAttackVariables
            {
                [SerializeField] private float chargeTimeMaxTime;
                [SerializeField] private float attackDuration;
                [SerializeField] private float maxStateTime;

                public float ChargeTimeMaxTime => chargeTimeMaxTime;
                public float AttackDuration => attackDuration;
                public float MaxStateTime => maxStateTime;
            }
            [System.Serializable]
            public class PlungeAttackVariables
            {
                [SerializeField] private float minHeight;
                [SerializeField] private float waitDuration;
                [SerializeField] private float minYVelocity;
                [SerializeField, BoundedCurve, Space(5)] private AnimationCurve speedUpCurve;
                [SerializeField] private float speedUpTime;
                [SerializeField] private float landDuration;

                public float MinHeight => minHeight;
                public float WaitDuration => waitDuration;
                public AnimationCurve SpeedUpCurve => speedUpCurve;
                public float SpeedUpTime => speedUpTime;
                public float MinYVelocity => minYVelocity;
                public float LandDuration => landDuration;
            }

            public BasicAttack1Variables BasicAttack1;
            public BasicAttack2Variables BasicAttack2;
            public BasicAttack3Variables BasicAttack3;
            public ChargeAttackVariables ChargeAttack;
            public PlungeAttackVariables PlungeAttack;
        }
    [System.Serializable]
    public class ShopVariables
    {
        [SerializeField] private float attackMultiplier;
        [SerializeField] private float horizontalSpeedMultiplier;
        [SerializeField] private float abilityPowerMultiplier;

        public float AttackMultiplier { get { return attackMultiplier; } set { attackMultiplier = value; } }
        public float HorizontalSpeedMultiplier { get { return horizontalSpeedMultiplier; } set { horizontalSpeedMultiplier = value; } }
        public float AbilityPowerMultiplier { get { return abilityPowerMultiplier; } set { abilityPowerMultiplier = value; } }
    }
        #endregion
        #endregion

        [Space(7)]
        public PhysicsVariables Physics;
        public WalkVariables Walk;
        public CrouchVariables Crouch;
        public JumpVariables Jump;
        public LandVariables Land;
        public DashVariables Dash;
        public SwingVariables Swing;
        public GlideVariables Glide;
        public AttackStateVariables Attack;
        public ShopVariables Shop;
        public WallMovementVariables Walls;
    }
}