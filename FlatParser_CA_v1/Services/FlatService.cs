using AutoMapper;
using DataAccess.Entity;
using DataAccess.Repository.Interface;
using FlatParser_CA_v1.Models;
using FlatParser_CA_v1.Services.Interfaces;

namespace FlatParser_CA_v1.Services
{
    public class FlatService : IFlatService
    {
        private IFlatRepository FlatRepository {  get; }
        private IMapper Mapper { get; }

        public FlatService(IFlatRepository flatRepository, IMapper mapper)
        {
            FlatRepository = flatRepository;
            Mapper = mapper;
        }

        public async Task<IEnumerable<FlatInfo>> GetFlats(CancellationToken cancellationToken)
        {
            var flats = await FlatRepository.GetFlats(cancellationToken) ?? throw new Exception("Flats is null.");

            return Mapper.Map<IEnumerable<FlatInfo>>(flats);
        }

        public async Task<bool> AddFlat(FlatInfo flat)
        {
            if (string.IsNullOrEmpty(flat.Address) || string.IsNullOrEmpty(flat.Link) || string.IsNullOrEmpty(flat.Price))
                throw new Exception("Entity is bad.");

            return await FlatRepository.AddFlat(Mapper.Map<FlatEntity>(flat));
        }

        public Task<bool> CheckFlatExists(string address, string link, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(link))
                throw new Exception("address or link is null.");

            return FlatRepository.CheckFlatExists(address, link, cancellationToken);
        }
    }
}
