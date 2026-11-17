using UnityEngine;
using VContainer;
using VContainer.Unity;
using static Game.Interactions.InteractionsKeys;

namespace Game.Interactions
{
    class GrabServiceClient : GrabServiceBase, ITickable, IStartable
    {
        public static void Install(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<GrabData>(lifetime);
            builder.RegisterEntryPoint<GrabServiceClient>(lifetime);
        }

        //todo: place this in GrabConfig
        //
        const float CLICK_DELAY = 1f;
        readonly LayerMask GRAB_LAYERS = LayerMask.GetMask("Grabbable");
        const float TRANSLATE_SPEED = 12f;
        const float ROTATION_SPEED = 12f;

        readonly GrabData _data;
        readonly IGrabDispatcher _dispatcher;
        readonly IGrabNetworkClient _network;
        readonly Collider _grabber;
        readonly Transform _gripPosition;

        float _clickTimer;
        Vector3 _gripOffset;

        public GrabServiceClient(
            GrabData data,
            IGrabDispatcher dispatcher,
            IGrabNetworkClient network,
            Collider grabber,
            [Key(GRIP_POSITION)] Transform gripPosition)
        {
            _data = data;
            _dispatcher = dispatcher;
            _network = network;
            _grabber = grabber;
            _gripPosition = gripPosition;
        }

        void IStartable.Start()
        {
            //consider that GrabClientService is singletone in IGrabNetworkClient scope 
            _network.GrabRequestAuthorized = Grab;
        }

        void ITickable.Tick()
        {
            if (_dispatcher.Grab)
            {
                if (_clickTimer > 0f)
                    _clickTimer -= Time.deltaTime;
                else
                {
                    if (!_data.grabbed)
                    {
                        Vector3 grabberPos = _grabber.transform.position;
                        Bounds bounds = _grabber.bounds;
                        float radius = GetCastRadius(bounds);

                        if (Physics.CapsuleCast(
                            new Vector3(grabberPos.x, bounds.max.y - radius, grabberPos.z),
                            new Vector3(grabberPos.x, bounds.min.y + radius, grabberPos.z),
                            radius,
                            _grabber.transform.forward,
                            out RaycastHit hit,
                            GetCastDistance(radius),
                            GRAB_LAYERS,
                            QueryTriggerInteraction.Ignore))
                        {
                            _network.GrabRequestRpc(hit.collider.gameObject); //invokes Grab method if success
                        }
                    }
                    else
                    {
                        Release(false);
                    }

                    _clickTimer = CLICK_DELAY;
                }
            }
            else if (_clickTimer > 0f)
                _clickTimer = 0f;

            if (_dispatcher.Throw && _data.grabbed)
            {
                Release(true);
            }

            if (_data.grabbed)
            {
                MoveGrabbed();
            }
        }

        private void Grab(GameObject grabbing)
        {
            _data.grabbed = grabbing.GetComponent<Collider>();

            //todo: Collide grabbed RB with other objects?
            if (_data.grabbed.attachedRigidbody)
                _data.grabbed.attachedRigidbody.isKinematic = true;

            Physics.IgnoreCollision(_data.grabbed, _grabber);

            _gripOffset = new(
                0f,
                _data.grabbed.bounds.extents.y,
                _data.grabbed.bounds.extents.z);
        }

        private void Release(bool withThrow)
        {
            Physics.IgnoreCollision(_data.grabbed, _grabber, false);

            if (withThrow)
                _network.GrabThrowRpc(_data.grabbed.gameObject);
            else
                _network.GrabReleaseRpc(_data.grabbed.gameObject);

            _data.grabbed = null;
        }

        private void MoveGrabbed()
        {
            _data.grabbed.transform.position = Vector3.MoveTowards(
                _data.grabbed.transform.position,
                _grabber.transform.TransformPoint(_gripPosition.localPosition + _gripOffset),
                TRANSLATE_SPEED * Time.deltaTime);

            //todo: preserve object's original rotation?
            _data.grabbed.transform.forward = Vector3.RotateTowards(
                _data.grabbed.transform.forward,
                _grabber.transform.forward,
                ROTATION_SPEED * Time.deltaTime,
                0f);
        }
    }
}
