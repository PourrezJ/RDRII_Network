#include "log.hpp"

namespace rh2::logging
{
    void Log::push(const std::string& prefix, const std::string& message)
    {
        auto msg = helpers::CreatePrefixedMessage(prefix, message);
        push(msg);
        std::cout << msg << std::endl;

    }
} // namespace rh2::logging
