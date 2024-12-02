using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSystem : MonoBehaviour
{
    #region Public Variables
    [field: Header("Movement")]
    [field: Tooltip("The speed that the player moves when on the ground.")]
    [field: SerializeField] private float MoveSpeed;

    [field: Tooltip("The speed that the player moves when scurrying on the ground.")]
    [field: SerializeField] private float ScurrySpeed;

    [field: Tooltip("The amount of time (in seconds) the player can scurry for before stopping")]
    [field: SerializeField] private float ScurryLimit;

    [field: Tooltip("The amount of time (in seconds) it takes for the player to recover their scurry ability")]
    [field: SerializeField] private float ScurryRecoveryTime;

    [field: Tooltip("The multiplier for how much the player's speed should be inhibited while pulling an object.\n\nNote that negative values will be flipped to be positive, should they be provided.")]
    [field: SerializeField] private float PullInhibitMultiplier;

    [field: Tooltip("The acceleration that will be applied to the player when they begin moving. Likewise, the time it takes for them to stop moving.")]
    [field: SerializeField] private float MoveEasing;

    [field: Tooltip("Locks the player's movement to a specific axis.")]
    [field: SerializeField] private MoveType MoveType = MoveType.TwoDimensionsOnly;

    [field: Tooltip("Unhook the player's movement from the camera.")]
    [field: SerializeField] private bool UnhookMovement = true;

    [field: Tooltip("Allows the player to jump for a short period of time after falling from a ledge.")]
    [field: SerializeField] private bool EnableCoyoteJump = true;

    [field: Tooltip("The duration of time that the player can fall for and coyote jump.")]
    [field: SerializeField] private float CoyoteTimer = 1.0f;

    [field: Header("Jumping & Gravity")]
    [field: Tooltip("The force that is applied to the player's y-axis upon hitting the jump key/button.")]
    [field: SerializeField] private float JumpForce;

    [field: Tooltip("The force that is applied to the player's z-axis upon hitting the jump key/button when climbing on a pipe.")]
    [field: SerializeField] private float JumpForcePipe;

    [field: Tooltip("Permits attempting a Scurry mid-air, better to be enabled to preserve game-flow.")]
    [field: SerializeField] private bool EnableScurryInAir;

    [field: Tooltip("When MoveType: `Two Dimensions Only` is Enabled, this prevents the Player from going off the Z-Axis.")]
    [field: SerializeField] private bool LockZAxis;

    [field: Tooltip("This is the world-space position that represents where the Player's Z-Axis will be locked to.")]
    [field: SerializeField] private float ZAxisPlane;

    [field: Tooltip("How much the gravity applied to the player is multiplied.")]
    [field: SerializeField] private float GravityMultiplier;

    [field: Tooltip("How much velocity the player should inherit while they're grounded.")]
    [field: SerializeField] private float VelocityYIdle = 0.0f;

    [field: Header("Climbing & Sliding")]
    [field: Tooltip("The speed that the player moves when climbing.")]
    [field: SerializeField] private float ClimbSpeed;

    [field: Tooltip("The speed at which the player will begin sliding. This force is eased onto the player and isn't instantaneous.")]
    [field: SerializeField] private float SlideSpeed;

    [field: Tooltip("How long the player has to be still on a pipe before they begin sliding.")]
    [field: SerializeField] private float TimeBeforeSlide;

    [field: Header("Lerping")]
    [field: SerializeField] private EasingStyle LerpStyle;
    [field: SerializeField] private float LerpSpeed;

    [field: Tooltip("The force that the player will push objects.")]
    [field: SerializeField] private float PushForce;

    [field: Header("Interacting")]
    [field: Tooltip("The GameObject Tag used to identify ALL Grabbable GameObjects within the current scene.")]
    [field: SerializeField] private string GrabTag = "Grabbable";

    [field: Header("Miscellaneous")]
    [field: SerializeField] private string SurfaceMaterialsPath = "SurfaceMaterials";
    [field: SerializeField] private SurfaceMaterial GenericSurface;

    [field: Tooltip("All the Surface Types that the Player can interact with")]
    private Dictionary<string, SurfaceMaterial> Surfaces = new();

    [field: Header("Externals")]
    [field: Tooltip("Reference to the camera that will follow the player.")]
    public CameraSystem Camera;
    [field: Tooltip("Reference to the player model's Animator that's used to animate the character.")]
    public Animator Animator;
    [field: Tooltip("The Character Model to rotate when dealing with rotations")]
    [field: SerializeField] private GameObject RotatableCharacter;

    [field: Header("Collections")]
    [field: SerializeField] private PlayerAnimation[] PlayerAnimations;
    #endregion

    #region Private Variables

    #region Public + Hidden Variables
    [HideInInspector] public bool ClimbingRequested;
    [HideInInspector] public bool IsClimbing;
    [HideInInspector] public bool IsJumpingFromClimb;
    [HideInInspector] public bool FallingFromClimb;

    [HideInInspector] public bool IsOnMop;

    [HideInInspector] public CharacterController Character;
    [HideInInspector] public bool IsHidden = false;

    [HideInInspector] public Vector3 WarpPosition;

    [HideInInspector] public PipeFunctionality CurrentPipe;
    [HideInInspector] public PipeSide CurrentPipeSide;

    [HideInInspector] public bool IsJumping;
    [HideInInspector] public bool JumpingRequested;
    [HideInInspector] public bool InteractHeld = false;

    [HideInInspector] public PlrCheckpoint CurrentCheckpoint;
    #endregion

    private Quaternion WarpRotation;
    private Quaternion CharacterRotation;

    private Vector2 MoveInput;
    private Vector3 MoveDelta;
    private Vector3 PullObjPos;
    private Vector3 HitDirection;

    private Vector3 Velocity;
    private Vector3 LastFrameVelocity = Vector3.zero;

    private CameraTarget OriginalSpawn;
    private SurfaceMaterial FloorMaterial;

    private BoxScript box;

    private bool UsedCoyoteJump;
    private float CurrentCoyoteTime;

    private float CurrentScurryTime;
    private float CurrentRecoveryTime;
    private float TimeUntilJumpButtonIsDisabled;

    private float SetMoveSpeed = 0.0f;
    private float MaterialMoveSpeed = 0.0f;

    private float CurrentMoveSpeed = 0.0f;
    private float PreviousMoveSpeed;

    private bool CanClimbUp = true, CanClimbDown = true;
    private float SlideTimer = 0.0f, SlideForce = 0.0f;

    private bool canGrab = true;
    private float grabDist;

    private bool CanScurry = true;
    private bool JumpButtonIsHeld = false;

    private bool IsScurrying, IsGrounded, IsMoving, IsSliding, IsBeingLaunched;
    private bool IsGrabbing, IsPushing, IsPulling;
    private bool dieByPray = false, dieByBurn = false, dieByDrown = false;
    #endregion

    #region Functions - Handlers
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (IsOnMop) { return; }
        MoveInput = ctx.ReadValue<Vector2>();
    }

    public void OnClimbing(InputAction.CallbackContext ctx)
    {
        if (MoveType == MoveType.None) return;
        ClimbingRequested = ctx.ReadValueAsButton();
    }

    public void OnScurry(InputAction.CallbackContext ctx)
    {
        if (MoveType == MoveType.None || !ctx.ReadValueAsButton() || ctx.phase != InputActionPhase.Performed ||
            IsClimbing || IsOnMop || IsGrabbing) return;

        IsScurrying = !IsScurrying;

        if (IsScurrying || !CanScurry || !IsMoving) return;
        CanScurry = false;
    }

    public void OnJumping(InputAction.CallbackContext ctx)
    {
        JumpingRequested = ctx.ReadValueAsButton();

        if (MoveType == MoveType.None) return;

        if ((IsClimbing && CurrentPipeSide == PipeSide.Left && MoveInput.x > 0) ||
            (IsClimbing && CurrentPipeSide == PipeSide.Right && MoveInput.x < 0) ||
            (IsGrabbing)) { return; }

        IsJumping = JumpingRequested;
    }

    public void OnInteracting(InputAction.CallbackContext ctx)
    {
        InteractHeld = false;
        if (ctx.phase != InputActionPhase.Performed) return;

        bool interactResult = ctx.ReadValueAsButton();

        if (canGrab) { InteractHeld = interactResult; }
        if (!interactResult) return;

        for (int i = 0; i < GameSystem.Instance.CachedInteractables.Count; i++)
        {
            IInteractableData data = GameSystem.Instance.CachedInteractables[i];
            data.Interactable.Interact(data.Parent, this);
        }
    }
    #endregion

    #region Functions - Public
    public Vector2 GetMoveInput() => MoveInput;
    public bool IsPlayerMoving() => IsMoving;
    public bool IsPlayerJumping() => IsJumping;
    public bool IsPlayerGrounded() => IsGrounded;
    public bool IsPlayerGrabbing() => IsGrabbing;
    public bool ToggleUpMovement(bool enable) => CanClimbUp = enable;
    public bool ToggleDownMovement(bool enable) => CanClimbDown = enable;
    public bool TogglePullState(bool input) => IsGrabbing = input;
    public bool ToggleCharCont(bool enable) => Character.enabled = enable;
    public bool ToggleGrabbing(bool enable) => canGrab = enable;

    public void Warp(Vector3 NewPosition) => WarpPosition = NewPosition;
    public void Warp(Vector3 NewPosition, Quaternion NewRotation)
    {
        WarpPosition = NewPosition;
        WarpRotation = NewRotation;
    }
    public void WarpTarget(CameraTarget NewTarget)
    {
        WarpPosition = NewTarget.position;
        WarpRotation = NewTarget.rotation;
    }
    public void WarpTarget(Transform NewTransform)
    {
        WarpPosition = NewTransform.position;
        WarpRotation = NewTransform.rotation;
    }

    public void SetVelocity(Vector3 NewVelocity) => Velocity = NewVelocity;

    public MoveType GetMoveType() => MoveType;
    public void SetMoveType(MoveType Type, bool ResetVelocity = false)
    {
        MoveType = Type;
        if (!ResetVelocity) return;
        SetVelocity(Vector3.zero);
    }

    public void ForcePlayerToJump(float forceToApply) => Velocity.y = forceToApply;
    public void LaunchPlayerFromMopBucket(float accuracy)
    {
        IsBeingLaunched = true;
        Velocity.x = 7 * accuracy;
    }

    public void ApplyImpulseToPlayer(float multiplier)
    {
        Velocity.x = 2 * multiplier;
    }

    public void PlayDeathAnimation(DeathType deathType)
    {
        switch (deathType)
        {
            case DeathType.Prayed:
                dieByPray = true;
                break;
            case DeathType.Burned:
                dieByBurn = true;
                break;
            case DeathType.Drowned:
                dieByDrown = true;
                break;
        }
    }

    public void DeathTriggered()
    {
        GameSystem.Instance.PlayerDiedCallback();
        RevertPlayerValues();

        bool checkpointResult = SpawnAtCheckpoint();
        if (checkpointResult) return;

        WarpTarget(OriginalSpawn);
    }
    #endregion

    #region Functions - Private
    private void RevertPlayerValues()
    {
        IsBeingLaunched = IsClimbing = IsGrabbing = IsHidden = IsJumping = IsJumpingFromClimb = IsMoving = IsOnMop = IsPulling = IsPushing = IsScurrying = IsSliding = false;
        dieByPray = dieByBurn = dieByDrown = false;
        MoveType = MoveType.TwoDimensionsOnly;
    }

    private bool SpawnAtCheckpoint()
    {
        if (!CurrentCheckpoint)
        {
            WarpTarget(OriginalSpawn);
            return false;
        }

        WarpTarget(CurrentCheckpoint.OverrideTransform == null ? CurrentCheckpoint.transform : CurrentCheckpoint.OverrideTransform);
        return true;
    }

    private void CalculateFloorMaterial()
    {
        if (IsGrounded)
        {
            PhysicMaterial physical = null;

            if (Physics.Raycast(new Ray(transform.position, Vector3.down), out RaycastHit hit)) physical = hit.collider.sharedMaterial;
            if (physical != null) Surfaces.TryGetValue(physical.name, out FloorMaterial);
            else FloorMaterial = GenericSurface;
        }
        else FloorMaterial = null;

        if (!FloorMaterial)
        {
            MaterialMoveSpeed = 0.0f;
            return;
        }

        if (FloorMaterial.PreventScurrying)
        {
            MaterialMoveSpeed = MoveSpeed;
            CanScurry = false;
        }

        switch (FloorMaterial.MathUsage)
        {
            case MathType.Addition: MaterialMoveSpeed += FloorMaterial.PlayerSpeedModifier; break;
            case MathType.Subtraction: MaterialMoveSpeed -= FloorMaterial.PlayerSpeedModifier; break;
            case MathType.Multiplication: MaterialMoveSpeed *= FloorMaterial.PlayerSpeedModifier; break;
            case MathType.Division: MaterialMoveSpeed /= FloorMaterial.PlayerSpeedModifier; break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent != null)
        {
            if (other.transform.parent.gameObject.CompareTag(GrabTag))
                grabDist = other.GetComponentInParent<BoxScript>().GetGrabDistance();
        }

        if (other.gameObject.CompareTag(GameSystem.Instance.TouchTag) != true) return;

        for (int i = 0; i < GameSystem.Instance.CachedTouchables.Count; i++)
        {
            ITouchableData data = GameSystem.Instance.CachedTouchables[i];

            if (data.Parent != other.gameObject) continue;
            data.Touchable.TriggerEnter(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(GameSystem.Instance.TouchTag))
        {
            for (int i = 0; i < GameSystem.Instance.CachedTouchables.Count; i++)
            {
                ITouchableData data = GameSystem.Instance.CachedTouchables[i];

                if (data.Parent != other.gameObject) continue;
                data.Touchable.TriggerStay(this);
            }
        }

        if (other.transform.parent != null) { if (!other.transform.parent.CompareTag(GrabTag)) { return; } }
        if (!TogglePullState(InteractHeld) || !canGrab) { return; }

        box = other.transform.parent.GetComponent<BoxScript>();
        box.UnfreezeBox();

        PullObjPos = other.transform.parent.position;

        // I know this is an atrocious way of checking which side of the object the player is on...
        // dw about it, I made it slightly less offensive lol

        float grabX = transform.position.x < other.transform.parent.position.x
            ? transform.position.x + grabDist // Left Side
            : transform.position.x - grabDist; // Right Side


        if (CharacterRotation.eulerAngles.y <= 360f && CharacterRotation.eulerAngles.y > 180f && transform.position.x <= PullObjPos.x)
        {
            if (!box.CheckIfValidPush(Vector3.right) && MoveInput.x > 0) { return; }
            other.transform.parent.position = new(grabX - other.transform.localPosition.x, other.transform.parent.position.y, other.transform.parent.position.z);
            return;
        }

        if (CharacterRotation.eulerAngles.y <= 180f && CharacterRotation.eulerAngles.y > 0f && transform.position.x > PullObjPos.x)
        {
            if (!box.CheckIfValidPush(Vector3.left) && MoveInput.x < 0) { return; }
            other.transform.parent.position = new(grabX - other.transform.localPosition.x, other.transform.parent.position.y, other.transform.parent.position.z);
            return;
        }


        IsPushing = IsPulling = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent != null)
        {
            if (other.transform.parent.CompareTag(GrabTag))
            { PullObjPos = Vector3.zero; IsPushing = false; IsPulling = false; IsGrabbing = false; return; }
        }

        if (other.gameObject.CompareTag(GameSystem.Instance.TouchTag) != true) return;

        for (int i = 0; i < GameSystem.Instance.CachedTouchables.Count; i++)
        {
            ITouchableData data = GameSystem.Instance.CachedTouchables[i];

            if (data.Parent != other.gameObject) continue;
            data.Touchable.TriggerLeave(this);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic) return;
        if (hit.moveDirection.y < -0.3f) return;

        Vector3 pushDir = new(hit.moveDirection.x, 0, hit.moveDirection.z);
        Vector3 collisionPoint = hit.point;

        body.AddForceAtPosition(pushDir * PushForce, collisionPoint, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        CalculateFloorMaterial();

        // TODO: This if statement chain needs to be improved for the sake of clarity!
        if (MaterialMoveSpeed <= 0.0f)
        {
            if ((!IsJumpingFromClimb && !IsScurrying && (!IsClimbing || !IsOnMop))) { SetMoveSpeed = MoveSpeed; }
            else if (IsJumpingFromClimb) { SetMoveSpeed = JumpForcePipe; }
            else if (!IsJumpingFromClimb && !IsScurrying && IsClimbing) { SetMoveSpeed = ClimbSpeed; }
            else if (IsOnMop) { SetMoveSpeed = 0; }
            else { SetMoveSpeed = ScurrySpeed; }
        }
        else SetMoveSpeed = MaterialMoveSpeed;

        Vector3 right = (!UnhookMovement) ? Camera.main.transform.right : Vector3.right;
        Vector3 forward = (!UnhookMovement) ? Camera.main.transform.forward : Vector3.forward;

        if ((!CanClimbUp && MoveInput.y > 0) || (!CanClimbDown && MoveInput.y < 0)) { MoveInput.y = 0; }

        MoveDelta = (MoveInput.x * right + MoveInput.y * forward) * SetMoveSpeed;

        if (IsJumping)
        {
            SlideTimer = 0;
            IsSliding = false;
            SlideForce = 0;

            // If IsJumping is true, set JumpButtonIsHeld to true after .05 seconds.
            TimeUntilJumpButtonIsDisabled += Time.fixedDeltaTime;
            if (TimeUntilJumpButtonIsDisabled > 0.05f) JumpButtonIsHeld = true;
        }

        if (IsClimbing)
        {
            IsJumpingFromClimb = false;

            if (MoveInput.y == 0) { if (SlideTimer < TimeBeforeSlide) { SlideTimer += Time.fixedDeltaTime; } }
            else { SlideTimer = 0; IsSliding = false; SlideForce = 0; }

            if (SlideTimer >= TimeBeforeSlide && CanClimbDown)
            {
                // Begin sliding downwards
                IsSliding = true;
                if (SlideForce > -2) { SlideForce -= Time.fixedDeltaTime; }
                MoveDelta.z = SlideForce;
            }

            if (IsBeingLaunched) { IsBeingLaunched = false; }
            if (IsScurrying) { IsScurrying = false; }

            if (!IsJumping && JumpButtonIsHeld)
            {
                JumpButtonIsHeld = false;
                TimeUntilJumpButtonIsDisabled = 0;
            }

            if (IsJumping && !JumpButtonIsHeld)
            {
                IsJumpingFromClimb = true;

                if (IsJumpingFromClimb && CurrentPipeSide == PipeSide.Right) { ApplyImpulseToPlayer(0.5f); }
                else if (IsJumpingFromClimb && CurrentPipeSide == PipeSide.Left) { ApplyImpulseToPlayer(-0.5f); }

                Velocity.y = JumpForce;
                IsClimbing = false;
            }
            else if (ClimbingRequested)
            {
                IsClimbing = false;
                FallingFromClimb = true;
            }
        }

        if (IsOnMop)
        {
            IsJumpingFromClimb = false;

            if (IsBeingLaunched) { IsBeingLaunched = false; }
        }

        Vector3 actualVelocity;

        if (!IsClimbing && !IsOnMop)
        {
            SlideTimer = 0;
            IsSliding = false;
            SlideForce = 0;

            if (!IsBeingLaunched)
            {
                Velocity.z = MoveDelta.z;
                Velocity.x = MoveDelta.x;
            }

            if (IsGrounded)
            {
                IsJumpingFromClimb = false;
                FallingFromClimb = false;
                IsBeingLaunched = false;

                if (!IsJumping && JumpButtonIsHeld) { JumpButtonIsHeld = false; TimeUntilJumpButtonIsDisabled = 0; }

                if (IsJumping && !FloorMaterial.PreventJumping && !JumpButtonIsHeld) Velocity.y = JumpForce;
                else if (Velocity.y < VelocityYIdle) Velocity.y = VelocityYIdle;

                IsPulling = false;
                IsPushing = false;

                if (IsGrabbing)
                {
                    if ((MoveInput.x < 0 && PullObjPos.x > transform.position.x) || (MoveInput.x > 0 && PullObjPos.x < transform.position.x))
                    { IsPulling = true; }
                    else { IsPulling = false; }


                    if ((MoveInput.x > 0 && PullObjPos.x > transform.position.x && box.CheckIfValidPush(Vector3.right)) || (MoveInput.x < 0 && PullObjPos.x < transform.position.x && box.CheckIfValidPush(Vector3.left)))
                    { IsPushing = true; }
                    else { IsPushing = false; }
                }
            }

            else
            {
                if (!EnableScurryInAir) { CanScurry = false; }

                if (EnableCoyoteJump && !UsedCoyoteJump && CurrentCoyoteTime < CoyoteTimer)
                {
                    CurrentCoyoteTime += Time.fixedDeltaTime;

                    if (IsJumping)
                    {
                        Velocity.y = JumpForce;
                        UsedCoyoteJump = true;
                    }
                }
            }

            Velocity += GravityMultiplier * Time.fixedDeltaTime * Physics.gravity;
            actualVelocity = Vector3.Lerp(LastFrameVelocity, Velocity, MoveEasing * Time.fixedDeltaTime);
        }
        else
        {
            Velocity.y = MoveDelta.z;

            actualVelocity = Vector3.Lerp(LastFrameVelocity, Velocity, 100 * Time.fixedDeltaTime);
            actualVelocity.z = 0;
        }

        PreviousMoveSpeed = CurrentMoveSpeed;
        CurrentMoveSpeed = new Vector2(actualVelocity.x, actualVelocity.z).magnitude;

        if (CurrentMoveSpeed < 0.0f) CurrentMoveSpeed = 0.0f;

        Vector3 importedVelocity = !IsGrabbing ? actualVelocity : actualVelocity / PullInhibitMultiplier;
        Character.Move(importedVelocity * Time.fixedDeltaTime);

        LastFrameVelocity = (!IsClimbing && !IsOnMop) ? new(actualVelocity.x, Velocity.y, actualVelocity.z) : new(actualVelocity.x, actualVelocity.y, Velocity.z);

        if (!IsGrounded) HitDirection = Vector3.zero;
        else { UsedCoyoteJump = false; CurrentCoyoteTime = 0.0f; }

        if (LockZAxis && MoveType == MoveType.TwoDimensionsOnly)
        {
            Vector3 task = Character.transform.position;
            task.z = ZAxisPlane;
            Character.transform.position = task;
        }

        if (!IsMoving)
        {
            Vector3 horizonalHitDirection = HitDirection;
            horizonalHitDirection.y = 0;

            float displacement = horizonalHitDirection.magnitude;
            if (displacement > 0.0f) Velocity -= 0.2f * horizonalHitDirection / displacement;

            return;
        }

        if (MoveInput.magnitude == 0) return;

        float radian = Mathf.Atan2(MoveInput.y, MoveInput.x * -1.0f);
        float degree = 180.0f * radian / Mathf.PI;
        float rotation = ((360.0f + Mathf.Round(degree)) % 360.0f) + 90.0f;

        if (!IsGrabbing)
        {
            CharacterRotation = Quaternion.Euler(
                0.0f, // Original: !IsClimbing ? 0.0f : 90.0f,
                !IsClimbing && !IsOnMop ? rotation : CurrentPipeSide == PipeSide.Left ? 220.0f : 140.0f,
                0.0f // Original: IsClimbing ? 180.0f : 0.0f,
            );
        }
        else
        {
            if (transform.position.x > PullObjPos.x) CharacterRotation = Quaternion.Euler(0, PullObjPos.y + 90, 0);
            else CharacterRotation = Quaternion.Euler(0, PullObjPos.y - 90, 0);
        }

        GameObject rotater = !RotatableCharacter ? Character.gameObject : RotatableCharacter;

        switch (LerpStyle)
        {
            case EasingStyle.Basic: CharacterRotation = Quaternion.RotateTowards(rotater.transform.rotation, CharacterRotation, Time.fixedDeltaTime * LerpSpeed); break;
            case EasingStyle.Lerp: CharacterRotation = Quaternion.Lerp(rotater.transform.rotation, CharacterRotation, Time.fixedDeltaTime * LerpSpeed); break;
            case EasingStyle.Slerp: CharacterRotation = Quaternion.Slerp(rotater.transform.rotation, CharacterRotation, Time.fixedDeltaTime * LerpSpeed); break;
        }

        rotater.transform.rotation = CharacterRotation;
    }

    private void Update()
    {
        switch (MoveType)
        {
            case MoveType.None: MoveInput = Vector2.zero; break;
            case MoveType.LockToLeftRight: MoveInput.y = 0.0f; break;
            case MoveType.LockToForwardBack: MoveInput.x = 0.0f; break;
            case MoveType.TwoDimensionsOnly: MoveInput.y = (!IsClimbing && !IsOnMop) ? 0.0f : MoveInput.y; break;
        }

        IsGrounded = Character.isGrounded;
        IsMoving = MoveDelta.magnitude != 0.0f;

        foreach (PlayerAnimation anim in PlayerAnimations)
        {
            if (anim.InternalAnimType != AnimType.Custom)
            {
                switch (anim.InternalAnimType)
                {
                    case AnimType.VerticalSpeed: Animator.SetFloat(anim.Name, Character.velocity.y); break;
                    case AnimType.HorizontalSpeed: Animator.SetFloat(anim.Name, CurrentMoveSpeed); break;
                    case AnimType.Jumping: Animator.SetBool(anim.Name, IsJumping && !IsGrounded); break;
                    case AnimType.Scurrying: Animator.SetBool(anim.Name, IsScurrying); break;
                    case AnimType.Climbing: Animator.SetBool(anim.Name, IsClimbing); break;
                    case AnimType.Sliding: Animator.SetBool(anim.Name, IsSliding); break;
                    case AnimType.Pushing: Animator.SetBool(anim.Name, IsPulling); break;
                    case AnimType.Pulling: Animator.SetBool(anim.Name, IsPushing); break;
                    case AnimType.Idle: Animator.SetBool(anim.Name, IsGrounded); break;
                    case AnimType.Burned: Animator.SetBool(anim.Name, dieByBurn); break;
                    case AnimType.Drowned: Animator.SetBool(anim.Name, dieByDrown); break;
                    case AnimType.Prayed: Animator.SetBool(anim.Name, dieByPray); break;
                    case AnimType.XVelocity: Animator.SetFloat(anim.Name, Mathf.Abs(Character.velocity.x)); break;
                }

                continue;
            }

            switch (anim.CustomValueType)
            {
                case AnimValueType.None:
                    Debug.LogWarning(name + " | Tried using Custom Value Parameter for Animation: " + anim.Name + ", however, the Custom Value Type was set to None!");
                    return;

                case AnimValueType.Integer:
                    if (!int.TryParse(anim.CustomValue, out int resultInt)) return;
                    Animator.SetInteger(anim.Name, resultInt);
                    break;

                case AnimValueType.Float:
                    if (!float.TryParse(anim.CustomValue, out float resultFloat)) return;
                    Animator.SetFloat(anim.Name, resultFloat);
                    break;

                case AnimValueType.Boolean:
                    if (!bool.TryParse(anim.CustomValue, out bool resultBool)) return;
                    Animator.SetBool(anim.Name, resultBool);
                    break;
            }
        }

        if (!canGrab) { InteractHeld = false; }

        if (IsScurrying)
        {
            if (CanScurry && CurrentScurryTime < ScurryLimit)
            {
                CurrentScurryTime += Time.deltaTime;
                return;
            }

            CanScurry = false;
            IsScurrying = false;

            return;
        }

        if (CanScurry) return;

        if (CurrentRecoveryTime < ScurryRecoveryTime)
        {
            CurrentRecoveryTime += Time.deltaTime;
            return;
        }

        CanScurry = true;

        CurrentScurryTime = 0.0f;
        CurrentRecoveryTime = 0.0f;
    }

    private void LateUpdate()
    {
        if (WarpPosition == Vector3.zero) return;

        Character.enabled = false;
        transform.position = WarpPosition;

        if (WarpRotation.eulerAngles != Vector3.zero)
        {
            transform.rotation = WarpRotation;
            CharacterRotation = WarpRotation;
        }

        Character.enabled = true;
        WarpRotation.eulerAngles = Vector3.zero;
        WarpPosition = Vector3.zero;
    }

    private void Start()
    {
        foreach (SurfaceMaterial material in Resources.LoadAll<SurfaceMaterial>(SurfaceMaterialsPath))
        {
            Surfaces.Add(material.Material.name, material);
        }

        OriginalSpawn = new()
        {
            position = transform.position,
            rotation = transform.rotation
        };

        if (PullInhibitMultiplier < 0) { PullInhibitMultiplier = -PullInhibitMultiplier; }

        SpawnAtCheckpoint();
    }

    private void Awake() => Character = GetComponent<CharacterController>();
    #endregion
}
