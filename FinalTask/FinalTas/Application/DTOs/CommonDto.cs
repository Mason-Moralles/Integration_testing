namespace FinalTas.Application.DTOs;

public record ErrorResponseDto(string Error, int Status);
public record PagedResultDto<T>(List<T> Items, int Page, int PageSize, int TotalCount, int TotalPages);
