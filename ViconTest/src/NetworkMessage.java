
public class NetworkMessage {

	private byte messageId;
	private byte[] content;
	private byte[] buffer = new byte[1000000];
	private int bufferIndex = 0;
	private int readIndex = 0;

	public byte getMessageId()
	{
		return messageId;
	}
	public byte[] getContent()
	{
		if (this.content == null)
		{
			this.content = new byte[this.bufferIndex + 1];

			for (int i = 0; i < this.bufferIndex + 1; ++i)
			{
				this.content[i] = this.buffer[i];
			}
		}
		return this.content;
	}
	
	public NetworkMessage(byte messageId)
    {
        this.messageId = messageId;
    }

    /// <summary>
    /// The constructor
    /// </summary>
    /// <param name="messageId">The ID of the message</param>
    /// <param name="content">The content of the message</param>
    public NetworkMessage(byte messageId, byte[] content)
    {
        this.messageId = messageId;
        this.content = content;
        this.buffer = content;
    }

}
