using UnityEngine;
using VContainer;
using VContainer.Unity;
using static Game.Interactions.InteractionsKeys;

namespace Game.Interactions
{
    class GrabServiceClient : GrabServiceBase, ITickable, IStartable
    {
        //todo: place this in GrabConfig
        //
        const float CLICK_DELAY = 1f;
        readonly LayerMask GRAB_LAYERS = LayerMask.GetMask("Grabbable");
        const float TRANSLATE_SPEED = 12f;
        const float ROTATION_SPEED = 12f;

        readonly IGrabDispatcher _dipatcher;
        readonly IGrabNetworkClient _network;
        readonly Collider _grabber;
        readonly Transform _gripPosition;

        float _clickTimer;
        Collider _grabbedObject;
        Vector3 _gripOffset;

        public GrabServiceClient(
            IGrabDispatcher dipatcher,
            IGrabNetworkClient network,
            Collider grabber,
            [Key(GRIP_POSITION)] Transform gripPosition)
        {
            _dipatcher = dipatcher;
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
            if (_dipatcher.Grab)
            {
                if (_clickTimer > 0f)
                    _clickTimer -= Time.deltaTime;
                else
                {
                    if (!_grabbedObject)
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

            if (_dipatcher.Throw && _grabbedObject)
            {
                Release(true);
            }

            if (_grabbedObject)
            {
                MoveGrabbed();
            }
        }

        private void Grab(GameObject grabbing)
        {
            _grabbedObject = grabbing.GetComponent<Collider>();

            //todo: Collide grabbed RB with other objects?
            if (_grabbedObject.attachedRigidbody)
                _grabbedObject.attachedRigidbody.isKinematic = true;

            Physics.IgnoreCollision(_grabbedObject, _grabber);

            _gripOffset = new(
                0f,
                _grabbedObject.bounds.extents.y,
                _grabbedObject.bounds.extents.z);
        }

        private void Release(bool withThrow)
        {
            Physics.IgnoreCollision(_grabbedObject, _grabber, false);

            if (withThrow)
                _network.GrabThrowRpc(_grabbedObject.gameObject);
            else
                _network.GrabReleaseRpc(_grabbedObject.gameObject);

            _grabbedObject = null;
        }

        private void MoveGrabbed()
        {
            _grabbedObject.transform.position = Vector3.MoveTowards(
                _grabbedObject.transform.position,
                _grabber.transform.TransformPoint(_gripPosition.localPosition + _gripOffset),
                TRANSLATE_SPEED * Time.deltaTime);

            //todo: preserve object's original rotation?
            _grabbedObject.transform.forward = Vector3.RotateTowards(
                _grabbedObject.transform.forward,
                _grabber.transform.forward,
                ROTATION_SPEED * Time.deltaTime,
                0f);
        }
    }
}
