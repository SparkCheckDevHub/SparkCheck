public class InterestSelectionDto {
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public List<InterestDto> Interests { get; set; } = new();
}

public class InterestDto {
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public bool IsSelected { get; set; } 
}
