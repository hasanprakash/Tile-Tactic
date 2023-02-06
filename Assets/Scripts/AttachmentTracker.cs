using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachmentTracker
{
    [HideInInspector] public string attachmentId;
    [HideInInspector] public int attachmentsCount = 0;
    [HideInInspector] public Vector3 currentPosition;
    [HideInInspector] public List<Attachment> attachments = new List<Attachment>();
    [HideInInspector] public bool occupied = false;
}
