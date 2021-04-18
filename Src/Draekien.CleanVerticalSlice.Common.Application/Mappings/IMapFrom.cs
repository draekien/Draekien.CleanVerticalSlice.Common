using AutoMapper;

namespace Draekien.CleanVerticalSlice.Common.Application.Mappings
{
    public interface IMapFrom<T>
    {
        /// <summary>
        /// Creates a default mapping profile from the source type T to the destination type (class tha implements this interface)
        /// </summary>
        /// <param name="profile">An AutoMapper <see cref="Profile"/></param>
        void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
    }
}
