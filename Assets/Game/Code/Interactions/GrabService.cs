using UnityEngine;
using VContainer;
using VContainer.Unity;
using static Game.Interactions.InteractionsKeys;

namespace Game.Interactions
{
    class GrabService : ITickable
    {
        //todo: place this in GrabConfig
        //
        readonly LayerMask GRAB_LAYERS = LayerMask.GetMask("Grabbable");
        readonly float TRANSLATE_SPEED = 12f;
        readonly float ROTATION_SPEED = 12f;
        readonly float THROW_FORCE = 10f;

        readonly IGrabDispatcher _dipatcher;
        readonly Collider _grabber;
        readonly Transform _gripPosition;

        Collider _grabbedObject;
        Vector3 _gripOffset;

        public GrabService(
            IGrabDispatcher dipatcher, 
            Collider grabber,
            [Key(GRIP_POSITION)] Transform gripPosition)
        {
            _dipatcher = dipatcher;
            _grabber = grabber;
            _gripPosition = gripPosition;
        }

        void ITickable.Tick()
        {
            if (_dipatcher.Grab && !_grabbedObject)
            {
                Vector3 grabberPos = _grabber.transform.position;
                Bounds bounds = _grabber.bounds;
                float radius = bounds.size.x * 0.3f;

                if (Physics.CapsuleCast(
                    new Vector3(grabberPos.x, bounds.max.y - radius, grabberPos.z),
                    new Vector3(grabberPos.x, bounds.min.y + radius, grabberPos.z),
                    radius,
                    _grabber.transform.forward,
                    out RaycastHit hit,
                    radius * 3.3f,
                    GRAB_LAYERS,
                    QueryTriggerInteraction.Ignore))
                {
                    Grab(hit.collider);
                }
            }

            if (!_dipatcher.Grab && _grabbedObject)
            {
                Release();
            }

            if (_dipatcher.Throw && _grabbedObject)
            {
                Throw(Release());
            }

            if (_grabbedObject)
            {
                MoveGrabbed();
            }
        }

        private void Grab(Collider @object)
        {
            _grabbedObject = @object;

            if (_grabbedObject.attachedRigidbody)
                _grabbedObject.attachedRigidbody.isKinematic = true;

            Physics.IgnoreCollision(_grabbedObject, _grabber);

            _gripOffset = new(
                0f,
                _grabbedObject.bounds.extents.y,
                _grabbedObject.bounds.extents.z);
        }

        private Collider Release()
        {
            if (_grabbedObject.attachedRigidbody)
                _grabbedObject.attachedRigidbody.isKinematic = false;

            Physics.IgnoreCollision(_grabbedObject, _grabber, false);

            var grabbedObj = _grabbedObject;
            _grabbedObject = null;
            return grabbedObj;
        }

        private void Throw(Collider @object)
        {
            if (@object.attachedRigidbody)
            {
                @object.attachedRigidbody.AddForce(
                    @object.transform.forward * THROW_FORCE,
                    ForceMode.Impulse
                );
            }
        }

        private void MoveGrabbed()
        {
            _grabbedObject.transform.position = Vector3.MoveTowards(
                _grabbedObject.transform.position,
                _grabber.transform.TransformPoint(_gripPosition.localPosition + _gripOffset),
                TRANSLATE_SPEED * Time.deltaTime);

            _grabbedObject.transform.forward = Vector3.RotateTowards(
                _grabbedObject.transform.forward,
                _grabber.transform.forward,
                ROTATION_SPEED * Time.deltaTime,
                0f);
        }
    }
}
