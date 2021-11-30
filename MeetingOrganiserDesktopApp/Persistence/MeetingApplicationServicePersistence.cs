using CommonData.DTOs;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MeetingOrganiserDesktopApp.Persistence
{
    public class MeetingApplicationServicePersistence : IMeetingApplicationPersistence
    {
        private HttpClient client;
        private ILogger log;

        public MeetingApplicationServicePersistence(String baseAddress)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);

            log = LogManager.GetLogger("service");
        }

        
        public MeetingApplicationServicePersistence(HttpClient client)
        {
            this.client = client;
            log = LogManager.GetLogger("service");
        }


        #region Reading


        public OrganisationDTO ReadOrganisationAsync(String organisationName)
        {
            try
            {
                log.Info("GET query on service " + client.BaseAddress + ", path: api/organisations/getorganisation/" + organisationName);

                HttpResponseMessage response = client.GetAsync("api/organisations/getorganisation/" + organisationName).Result; // blocking call, should NOT use await!!
                if (response.IsSuccessStatusCode)
                {
                    var organisationDTO = response.Content.ReadAsAsync<OrganisationDTO>().Result;                               // blocking call, should NOT use await!!

                    response = client.GetAsync("api/projects/getprojectsoforganisation/" + organisationDTO.Id).Result;          // blocking call, should NOT use await!!
                    if (response.IsSuccessStatusCode)
                    {
                        organisationDTO.Projects = response.Content.ReadAsAsync<IEnumerable<ProjectDTO>>().Result.ToList();     // blocking call, should NOT use await!!
                    }

                    response = client.GetAsync("api/jobs/getjobs/" + organisationDTO.Id).Result;                                // blocking call, should NOT use await!!
                    if (response.IsSuccessStatusCode)
                    {
                        organisationDTO.Jobs = response.Content.ReadAsAsync<IEnumerable<JobDTO>>().Result.ToList();             // blocking call, should NOT use await!!
                    }

                    response = client.GetAsync("api/acceptedemaildomains/getacceptedemaildomains/" + organisationDTO.Id).Result;          // blocking call, should NOT use await!!
                    if (response.IsSuccessStatusCode)
                    {
                        organisationDTO.AcceptedEmailDomains = response.Content.ReadAsAsync<IEnumerable<AcceptedEmailDomainDTO>>().Result.ToList();          // blocking call, should NOT use await!!
                    }

                    return organisationDTO;
                }
                else
                {
                    log.Warn("GET query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                    throw new PersistenceUnavailableException("Service returned response: " + response.StatusCode);
                }
            }
            catch (PersistenceUnavailableException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                log.Error(ex, "GET query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }
        

        public async Task<IEnumerable<MemberDTO>> ReadMembersAsync(int organisationId)
        {
            try
            {
                log.Info("GET query on service " + client.BaseAddress + ", path: api/members/getmembers/" + organisationId);

                HttpResponseMessage response = await client.GetAsync("api/members/getmembers/" + organisationId);
                if (response.IsSuccessStatusCode)
                {
                    var memberDTOs =  response.Content.ReadAsAsync<IEnumerable<MemberDTO>>().Result;

                    foreach (var memberDTO in memberDTOs)
                    {
                        response = await client.GetAsync("api/projects/getprojectsofmember/" + memberDTO.Id);
                        if (response.IsSuccessStatusCode)
                        {
                            memberDTO.Projects = (await response.Content.ReadAsAsync<IEnumerable<ProjectDTO>>()).ToList();
                        }

                        response = await client.GetAsync("api/members/getmember/" + memberDTO.BossId);
                        if (response.IsSuccessStatusCode)
                        {
                            memberDTO.Boss = (await response.Content.ReadAsAsync<MemberDTO>());
                        }

                        response = await client.GetAsync("api/jobs/getjob/" + memberDTO.JobId);
                        if (response.IsSuccessStatusCode)
                        {
                            memberDTO.Job = (await response.Content.ReadAsAsync<JobDTO>());
                        }
                    }

                    return memberDTOs;
                }
                else
                {
                    log.Warn("GET query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                    throw new PersistenceUnavailableException("Service returned response: " + response.StatusCode);
                }
            }
            catch (PersistenceUnavailableException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                log.Error(ex, "GET query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }


        public async Task<IEnumerable<EventDTO>> ReadEventsAsync(int organisationId)
        {
            try
            {
                log.Info("GET query on service " + client.BaseAddress + ", path: api/events/getevents/" + organisationId);

                HttpResponseMessage response = await client.GetAsync("api/events/getevents/" + organisationId);
                if (response.IsSuccessStatusCode)
                {
                    var eventDTOs = response.Content.ReadAsAsync<IEnumerable<EventDTO>>().Result;

                    foreach (var eventDTO in eventDTOs)
                    {
                        eventDTO.Venues = (await ReadVenuesAsync(eventDTO.Id)).ToList();
                    }

                    return eventDTOs;
                }
                else
                {
                    log.Warn("GET query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                    throw new PersistenceUnavailableException("Service returned response: " + response.StatusCode);
                }
            }
            catch (PersistenceUnavailableException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                log.Error(ex, "GET query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }


        public async Task<IEnumerable<VenueDTO>> ReadVenuesAsync(int eventId)
        {
            try
            {
                log.Info("GET query on service " + client.BaseAddress + String.Format(", path: api/events/getvenues/{0}", eventId));

                HttpResponseMessage response = await client.GetAsync(String.Format("api/venues/getvenues/{0}", eventId));
                if (response.IsSuccessStatusCode)
                {
                    var venueDTOs = response.Content.ReadAsAsync<IEnumerable<VenueDTO>>().Result;

                    foreach (var venueDTO in venueDTOs)
                    {
                        response = await client.GetAsync("api/venueimages/getvenueimages/" + venueDTO.Id);
                        if (response.IsSuccessStatusCode)
                        {
                            venueDTO.Images = (await response.Content.ReadAsAsync<IEnumerable<VenueImageDTO>>()).ToList();
                        }
                    }

                    return venueDTOs;
                }
                else
                {
                    log.Warn("GET query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                    throw new PersistenceUnavailableException("Service returned response: " + response.StatusCode);
                }
            }
            catch (PersistenceUnavailableException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                log.Error(ex, "GET query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }


        /*
        public async Task<IEnumerable<AcceptedEmailDomainDTO>> ReadAcceptedEmailDomainsAsync(int organisationId)
        {
            try
            {
                log.Info("GET query on service " + client.BaseAddress + String.Format(", path: api/events/getacceptedemaildomains/{0}", organisationId));

                HttpResponseMessage response = await client.GetAsync(String.Format("api/events/getacceptedemaildomains/{0}", organisationId));
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IEnumerable<AcceptedEmailDomainDTO>>();
                }
                else
                {
                    log.Warn("GET query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                    throw new PersistenceUnavailableException("Service returned response: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "GET query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }

        public async Task<IEnumerable<JobDTO>> ReadJobsAsync(int organisationId)
        {
            try
            {
                log.Info("GET query on service " + client.BaseAddress + String.Format(", path: api/events/getjobs/{0}", organisationId));

                HttpResponseMessage response = await client.GetAsync(String.Format("api/events/getjobs/{0}", organisationId));
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IEnumerable<JobDTO>>();
                }
                else
                {
                    log.Warn("GET query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                    throw new PersistenceUnavailableException("Service returned response: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "GET query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }

        public async Task<IEnumerable<ProjectDTO>> ReadProjectsAsync(int organisationId)
        {
            try
            {
                log.Info("GET query on service " + client.BaseAddress + String.Format(", path: api/events/getprojects/{0}", organisationId));

                HttpResponseMessage response = await client.GetAsync(String.Format("api/events/getprojects/{0}", organisationId));
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IEnumerable<ProjectDTO>>();
                }
                else
                {
                    log.Warn("GET query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                    throw new PersistenceUnavailableException("Service returned response: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "GET query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }

        public async Task<IEnumerable<VenueImageDTO>> ReadVenueImagesAsync(int venueId)
        {
            try
            {
                log.Info("GET query on service " + client.BaseAddress + String.Format(", path: api/events/getvenueimages/{0}", venueId));

                HttpResponseMessage response = await client.GetAsync(String.Format("api/events/getvenueimages/{0}", venueId));
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IEnumerable<VenueImageDTO>>();
                }
                else
                {
                    log.Warn("GET query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                    throw new PersistenceUnavailableException("Service returned response: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "GET query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }
        */


        #endregion



        #region Updating


        public async Task<Boolean> UpdateMemberAsync(MemberDTO member)
        {
            try
            {
                log.Info("PUT query on service " + client.BaseAddress + ", path: api/members/putmember");
                log.Trace("PUT query content: " + JsonConvert.SerializeObject(member, Formatting.None));

                HttpResponseMessage response = await client.PutAsJsonAsync("api/members/putmember", member);

                if (!response.IsSuccessStatusCode)
                    log.Warn("PUT query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "PUT query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }


        public async Task<Boolean> UpdateOrganisationAsync(OrganisationDTO organisation)
        {
            try
            {
                log.Info("PUT query on service " + client.BaseAddress + ", path: api/organisations/putorganisation");
                log.Trace("PUT query content: " + JsonConvert.SerializeObject(organisation, Formatting.None));

                HttpResponseMessage response = await client.PutAsJsonAsync("api/organisations/putorganisation", organisation);

                if (!response.IsSuccessStatusCode)
                    log.Warn("PUT query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "PUT query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }


        public async Task<Boolean> UpdateEventAsync(EventDTO @event)
        {
            try
            {
                log.Info("PUT query on service " + client.BaseAddress + ", path: api/events/putevent");
                log.Trace("PUT query content: " + JsonConvert.SerializeObject(@event, Formatting.None));

                HttpResponseMessage response = await client.PutAsJsonAsync("api/events/putevent", @event);

                if (!response.IsSuccessStatusCode)
                    log.Warn("PUT query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "PUT query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }


        public async Task<Boolean> UpdateVenueAsync(VenueDTO venue)
        {
            try
            {
                log.Info("PUT query on service " + client.BaseAddress + ", path: api/venues/putvenue");
                log.Trace("PUT query content: " + JsonConvert.SerializeObject(venue, Formatting.None));

                HttpResponseMessage response = await client.PutAsJsonAsync("api/venues/putvenue", venue);

                if (!response.IsSuccessStatusCode)
                    log.Warn("PUT query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "PUT query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }


        public async Task<Boolean> UpdateVenueImageAsync(VenueImageDTO image)
        {
            try
            {
                log.Info("PUT query on service " + client.BaseAddress + ", path: api/venueimages/putvenueimage");
                log.Trace("PUT query content: " + JsonConvert.SerializeObject(image, Formatting.None));

                HttpResponseMessage response = await client.PutAsJsonAsync("api/venueimages/putvenueimage", image);

                if (!response.IsSuccessStatusCode)
                    log.Warn("PUT query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "PUT query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }


        public async Task<Boolean> UpdateAcceptedEmailDomainAsync(AcceptedEmailDomainDTO acceptedEmailDomain)
        {
            try
            {
                log.Info("PUT query on service " + client.BaseAddress + ", path: api/acceptedemaildomains/putacceptedemaildomain");
                log.Trace("PUT query content: " + JsonConvert.SerializeObject(acceptedEmailDomain, Formatting.None));

                HttpResponseMessage response = await client.PutAsJsonAsync("api/acceptedemaildomains/putacceptedemaildomain", acceptedEmailDomain);

                if (!response.IsSuccessStatusCode)
                    log.Warn("PUT query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "PUT query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }



        #endregion



        #region Deleting


        public async Task<Boolean> DeleteMemberAsync(MemberDTO member)
        {
            try
            {
                log.Info("DELETE query on service " + client.BaseAddress + ", path: api/members/deletemember/" + member.Id);
                log.Trace("DELETE query content: " + JsonConvert.SerializeObject(member, Formatting.None));

                HttpResponseMessage response = await client.DeleteAsync("api/members/deletemember/" + member.Id);

                if (!response.IsSuccessStatusCode)
                    log.Warn("DELETE query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "DELETE query aborted with exception.");

                throw new PersistenceUnavailableException(ex);
            }
        }



        public async Task<Boolean> DeleteOrganisationAsync(OrganisationDTO organisation)
        {
            try
            {
                log.Info("DELETE query on service " + client.BaseAddress + ", path: api/organisations/deleteorganisation");
                log.Trace("DELETE query content: " + JsonConvert.SerializeObject(organisation, Formatting.None));

                HttpResponseMessage response = await client.DeleteAsync("api/organisations/deleteorganisation/" + organisation.Id);

                if (!response.IsSuccessStatusCode)
                    log.Warn("DELETE query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "DELETE query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }


        public async Task<Boolean> DeleteEventAsync(EventDTO @event)
        {
            try
            {
                log.Info("DELETE query on service " + client.BaseAddress + ", path: api/events/deleteevent");
                log.Trace("DELETE query content: " + JsonConvert.SerializeObject(@event, Formatting.None));

                HttpResponseMessage response = await client.DeleteAsync("api/events/deleteevent/" + @event.Id);

                if (!response.IsSuccessStatusCode)
                    log.Warn("DELETE query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "DELETE query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }


        public async Task<Boolean> DeleteVenueAsync(VenueDTO venue)
        {
            try
            {
                log.Info("DELETE query on service " + client.BaseAddress + ", path: api/venues/deletevenue");
                log.Trace("DELETE query content: " + JsonConvert.SerializeObject(venue, Formatting.None));

                HttpResponseMessage response = await client.DeleteAsync("api/venues/deletevenue/" + venue.Id);

                if (!response.IsSuccessStatusCode)
                    log.Warn("DELETE query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "DELETE query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }


        public async Task<Boolean> DeleteVenueImageAsync(VenueImageDTO image)
        {
            try
            {
                log.Info("DELETE query on service " + client.BaseAddress + ", path: api/venueimages/deletevenueimage");
                log.Trace("DELETE query content: " + JsonConvert.SerializeObject(image, Formatting.None));

                HttpResponseMessage response = await client.DeleteAsync("api/venueimages/deletevenueimage/" + image.Id);

                if (!response.IsSuccessStatusCode)
                    log.Warn("DELETE query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "DELETE query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }


        public async Task<Boolean> DeleteAcceptedEmailDomainAsync(AcceptedEmailDomainDTO acceptedEmailDomain)
        {
            try
            {
                log.Info("DELETE query on service " + client.BaseAddress + ", path: api/acceptedemaildomains/deleteacceptedemaildomain");
                log.Trace("DELETE query content: " + JsonConvert.SerializeObject(acceptedEmailDomain, Formatting.None));

                HttpResponseMessage response = await client.DeleteAsync(
                    "api/acceptedemaildomains/deleteacceptedemaildomain/" + 
                    acceptedEmailDomain.OrganisationId + "/" + 
                    acceptedEmailDomain.DomainName);

                if (!response.IsSuccessStatusCode)
                    log.Warn("DELETE query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "DELETE query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }


        public async Task<Boolean> DeleteJobAsync(JobDTO job)
        {
            try
            {
                log.Info("DELETE query on service " + client.BaseAddress + ", path: api/jobs/deletejob");
                log.Trace("DELETE query content: " + JsonConvert.SerializeObject(job, Formatting.None));

                HttpResponseMessage response = await client.DeleteAsync ("api/jobs/deletejob/" + job.Id);

                if (!response.IsSuccessStatusCode)
                    log.Warn("DELETE query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "DELETE query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }

        #endregion



        #region Creating


        public async Task<Boolean> CreateMemberAsync(MemberDTO member)
        {
            try
            {
                log.Info("POST query on service " + client.BaseAddress + ", path: api/members/postmember");
                var serializedMember = JsonConvert.SerializeObject(member, Formatting.None);
                log.Trace("POST query content: " + serializedMember);

                HttpResponseMessage response = await client.PostAsJsonAsync("api/members/postmember", member);
                member.Id = (await response.Content.ReadAsAsync<MemberDTO>()).Id;

                if (!response.IsSuccessStatusCode)
                    log.Warn("POST query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "POST query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }


        public async Task<Boolean> CreateOrganisationAsync(OrganisationDTO organisation)
        {
            try
            {
                log.Info("POST query on service " + client.BaseAddress + ", path: api/organisations/postorganisation");
                log.Trace("POST query content: " + JsonConvert.SerializeObject(organisation, Formatting.None));

                HttpResponseMessage response = await client.PostAsJsonAsync("api/organisations/postorganisation", organisation);
                organisation.Id = (await response.Content.ReadAsAsync<OrganisationDTO>()).Id;

                if (!response.IsSuccessStatusCode)
                    log.Warn("POST query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "POST query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }


        public async Task<Boolean> CreateEventAsync(EventDTO @event)
        {
            try
            {
                log.Info("POST query on service " + client.BaseAddress + ", path: api/events/postevent");
                log.Trace("POST query content: " + JsonConvert.SerializeObject(@event, Formatting.None));

                HttpResponseMessage response = await client.PostAsJsonAsync("api/events/postevent", @event);
                @event.Id = (await response.Content.ReadAsAsync<EventDTO>()).Id;

                if (!response.IsSuccessStatusCode)
                    log.Warn("POST query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "POST query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }


        public async Task<Boolean> CreateVenueAsync(VenueDTO venue)
        {
            try
            {
                log.Info("POST query on service " + client.BaseAddress + ", path: api/venues/postvenue");
                log.Trace("POST query content: " + JsonConvert.SerializeObject(venue, Formatting.None));

                HttpResponseMessage response = await client.PostAsJsonAsync("api/venues/postvenue", venue);
                venue.Id = (await response.Content.ReadAsAsync<VenueDTO>()).Id;

                if (!response.IsSuccessStatusCode)
                    log.Warn("POST query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "POST query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }


        public async Task<Boolean> CreateVenueImageAsync(VenueImageDTO image)
        {
            try
            {
                log.Info("POST query on service " + client.BaseAddress + ", path: api/venueimages/postvenueimage");
                log.Trace("POST query content: " + JsonConvert.SerializeObject(image, Formatting.None));

                HttpResponseMessage response = await client.PostAsJsonAsync("api/venueimages/postvenueimage", image);
                image.Id = (await response.Content.ReadAsAsync<VenueImageDTO>()).Id;

                if (!response.IsSuccessStatusCode)
                    log.Warn("POST query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "POST query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }


        public async Task<Boolean> CreateAcceptedEmailDomainAsync(AcceptedEmailDomainDTO acceptedEmailDomain)
        {
            try
            {
                log.Info("POST query on service " + client.BaseAddress + ", path: api/acceptedemaildomains/postacceptedemaildomain");
                log.Trace("POST query content: " + JsonConvert.SerializeObject(acceptedEmailDomain, Formatting.None));

                HttpResponseMessage response = await client.PostAsJsonAsync("api/acceptedemaildomains/postacceptedemaildomain", acceptedEmailDomain);

                if (!response.IsSuccessStatusCode)
                    log.Warn("POST query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "POST query aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }



        #endregion



        public async Task<Boolean> LoginAsync(String userName, String userPassword, String organisationName)
        {
            try
            {
                //log.Info("LOGIN on service {0}, path: api/account/login/, user name: {1}", client.BaseAddress, userName);

                string request = "api/account/login/" + userName + "/" + userPassword + "/" + organisationName;
                HttpResponseMessage response = await client.GetAsync("api/account/login/" + userName + "/" + userPassword + "/" + organisationName);

                if (!response.IsSuccessStatusCode)
                    log.Warn("LOGIN returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "LOGIN aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }


        public async Task<Boolean> LogoutAsync()
        {
            try
            {
                log.Info("LOGOUT on service " + client.BaseAddress + ", path: api/account/logout/");

                HttpResponseMessage response = await client.GetAsync("api/account/logout");

                if (!response.IsSuccessStatusCode)
                    log.Warn("LOGOUT returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return !response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log.Error(ex, "LOGOUT aborted with exception.");
                throw new PersistenceUnavailableException(ex);
            }
        }
    }
}
